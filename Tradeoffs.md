# Trade-offs & Architectural Decisions

This document explains the *why* behind decisions made in this codebase — including ones that were tried, found wanting, and reversed. The goal is to leave a trail of reasoning, not just a snapshot of the current state.

## Repository Pattern: `IRepositoryManager` over per-entity repositories

**Decision:** Kept a single `IRepositoryManager` aggregator over per-entity repositories, all sharing one `ApplicationDbContext`. Each repository exposes a generic `Get(predicate, trackChanges)` returning `IQueryable<T>`. Query handlers chain `.Select()` onto `Get()` to build DTO projections themselves — repositories never contain projection/DTO-shaping logic.

**Why:** An earlier iteration had repository methods like `GetAllProperties()` that hardcoded a specific DTO shape. This became a problem as soon as a second consumer needed a slightly different shape of the same data — the repository method was either duplicated or bent out of shape to serve two masters. Moving projection logic into the handler (where the DTO requirement actually lives) keeps repositories generic and reusable, and keeps each query handler responsible for exactly the data shape it needs.

**Trade-off accepted:** Query handlers use `_repositoryManager.X.Get(predicate).Select(...)` rather than a separate `IApplicationDbContext` abstraction. An `IApplicationDbContext` interface was considered and rejected, in favor of staying consistent with one data-access path everywhere, including reads — fewer concepts to maintain, at the cost of query handlers being slightly more verbose than they'd be with direct DbContext access.

## Dropping the `UnavailableDate` table (a deliberate YAGNI call)

**Decision:** Removed a previously-implemented `UnavailableDate` table that handled soft/hard date blocking with a unique constraint. Booking overlap detection now queries the `Bookings` table directly.

**Why:** At the project's current and projected scale (~10k users), the probability of two tenants racing to book the same property on the same dates within the same few-hundred-millisecond window is low enough that a dedicated blocking table — with its own consistency requirements — wasn't justified. This was a conscious choice to not build for a scale or contention level the project doesn't have yet.

**Trade-off accepted:** Safeguards against double-booking are now applied only at the moment money moves (booking confirmation / payment), not at initial booking creation. This means a genuine race condition at creation time is *possible* in theory — see the Concurrency section below for how this was actually tested and what it revealed.

## Concurrency strategy: compare-and-swap, not pessimistic locking

**Decision:** No `FOR UPDATE` locks or database-level unique constraints were adopted for booking state transitions. The one concurrency pattern in active use is a **conditional atomic update** via EF Core's `ExecuteUpdateAsync`, with a `WHERE` clause checking the current status before transitioning — a `rowsAffected == 0` result means another process already changed the row first.

**Why this isn't optimistic concurrency (xmin/rowversion):** This is a meaningful distinction worth being explicit about. Optimistic concurrency (e.g. EF Core's `xmin` tracking) detects *any* change to a row since it was last read — a generic "has this row changed at all" check. The CAS pattern used here instead encodes a specific *business-state precondition* directly into the UPDATE statement itself: "only transition this booking if it is still `AwaitingConfirmation`." The database enforces this atomically — there's no window between checking the condition and applying the change where another process could interleave.

**Where this is used:** the payment-webhook-vs-expiry-job race. A booking awaiting payment can be confirmed by Paystack's webhook at the same moment the `AwaitingPaymentExpiryJob` decides it has timed out. Both paths attempt a conditional `UPDATE ... WHERE Status = 'AwaitingConfirmation'`; whichever commits first wins, and the loser's `rowsAffected == 0` tells it to back off — logged as `Information`, not an error, since this is an expected and successfully-handled race, not a bug.

**A real gap this approach left, and how it was found:** the `CreateBookingCommandHandler`'s date-conflict check (`HasConflictBookingAsync`) was originally called once before the transaction and once inside it — but under Postgres's default `Read Committed` isolation level, two concurrent transactions can both pass this check before either commits, since neither sees the other's uncommitted insert. This means two overlapping bookings *could* both be created. This was identified as a known risk during development but not yet been proven or fixed with a real concurrency test; see Known Gaps below.

## Booking status lifecycle

**Decision:** `Pending → AwaitingPayment → Confirmed → Completed`, with `Cancelled`/`Rejected` as exits.

**Why `AwaitingPayment` exists as its own state:** without it, a landlord-confirmed booking could be treated as "locked in" before payment was ever collected — creating a refund-without-payment vulnerability if a tenant later disputed or cancelled. Separating "landlord agreed" from "tenant paid" closes that gap.

## Layering discipline: provider-specific exceptions stay in Infrastructure

**Decision:** Postgres-specific exceptions (`PostgresException`, `Npgsql`-namespaced types) are caught and translated inside the Infrastructure layer only, never referenced in Application. The pattern: Infrastructure catches `DbUpdateException`, inspects `SqlState`, and throws a clean Application-layer exception (e.g. `DateConflictException`) instead.

**Why:** Application-layer code shouldn't need to know or care which database engine is underneath it. If the project ever needed to swap databases, or if a handler's unit tests need to simulate a conflict, they can work against a clean, provider-agnostic exception type rather than a Postgres-specific one.

## Background jobs: interface-first registration

**Decision:** Each Hangfire job (`BookingAutoExpiryJob`, `AwaitingPaymentExpiryJob`, `CheckoutJob`) is registered behind its own interface (`IBookingAutoExpiryJob`, etc.) via `services.AddScoped<IXJob, XJob>()`, and Hangfire resolves the job through DI rather than registering the concrete class directly.

**Why:** Primarily for testability and substitutability — though as of this writing, the interfaces exist but the jobs themselves are still only unit-testable around their decision logic (the CAS skip/proceed branching), not their actual database interaction, which is reserved for integration tests.

## Testing strategy: unit tests for decision logic, integration tests for real persistence and concurrency

**Decision:** Unit tests (xUnit + Moq + MockQueryable) cover `CreateBookingCommandHandler`'s full decision tree — every validation rule, every authorization check, the conflict-detection branch, and the success path — using mocked repositories. No real database is touched in this test suite.

**Why this scope, and not more:** this is a learning project as much as a portfolio piece. Rather than mechanically chase 100% handler coverage across the whole codebase, testing effort was deliberately scoped to one handler, exercised thoroughly, as a demonstration of testing methodology — arranging mocks correctly, tracing execution paths to know what needs mocking and what doesn't, and using tests as a forcing function to find real bugs rather than just decoration after the fact.

**A real bug this caught:** while writing the success-path test for `CreateBookingCommandHandler`, the date-conflict check turned out to be inverted — `HasConflictBookingAsync` returning `true` means "a conflict exists," but the handler's pre-transaction check read `if (!isDateAvailable) return Failure(...)`, treating the return value backwards. A passing test for this exact line would have meant new bookings were being rejected whenever dates were actually free, or worse, allowed through when dates were genuinely conflicting, depending on which of the two (inconsistent) call sites executed. Writing the test forced tracing the method's actual contract against its actual usage, and the mismatch was unambiguous once both were written down side by side. This is recorded here deliberately, not edited out, because it's a more honest demonstration of why testing matters than a perfect, scrubbed history would be.

**What's intentionally left to integration tests, not yet built:**
- The actual concurrent-booking race condition described above — proving (or disproving) that two simultaneous `CreateBookingCommandHandler` calls for the same property and overlapping dates correctly result in exactly one success and one failure. This requires a real Postgres instance with real transaction isolation, which unit tests with mocks cannot simulate, since mocks have no concept of true concurrent execution or commit ordering.
- Real EF Core query translation correctness (e.g., confirming `HasConflictBookingAsync`'s LINQ expression actually produces the overlap condition intended, rather than trusting a mock to always return what a test tells it to).
- Background job behavior against a real database, beyond the CAS branching logic already covered by unit tests.

Setting up integration tests against a real, disposable Postgres instance (likely via Testcontainers) is the next planned step, pending hands-on Docker/container fundamentals — sequencing infrastructure tooling knowledge before relying on it in tests, rather than copying setup steps without understanding what's actually running underneath.

## Known gaps / open threads

- The pre-transaction vs. in-transaction double conflict-check in `CreateBookingCommandHandler` is currently redundant in intent but was, until recently, inconsistent in implementation (only one of the two call sites had the inversion bug fixed first). Worth revisiting whether both checks are still necessary, or whether the pre-transaction check should be removed entirely in favor of relying solely on the in-transaction one.
- No `GetBookingByIdQuery` exists yet.
- Payment system (Paystack integration, webhook handling, refund logic) is designed (see internal `payment.md` notes) but not yet implemented. Key open question: how to detect and reconcile a payment that succeeds for a booking the expiry job has already cancelled — current direction is a periodic reconciliation job that verifies transaction status directly against Paystack, rather than trusting webhooks as the sole source of truth.
- `CheckoutJob`'s payout calculation (`AmountKobo = SubtotalKobo`, landlord does not receive the platform fee) should be confirmed against actual business intent before going live.