<<<<<<< HEAD
# letiahomes
=======
# letiahomes-Backend
A shortlet rental booking platform API built with .NET Core, designed around Clean Architecture principles with a focus on correctness under concurrency — race-safe booking state transitions
## Live API Docs
 
Once running locally, full interactive API documentation (every endpoint, request/response schemas, auth requirements) is available via Swagger UI:
 
```
https://localhost:7072/swagger
```
This README covers the *why* behind the system's design. For the *what* — exact routes, payloads, status codes — Swagger is the source of truth and is generated directly from the code, so it can never drift out of date the way hand-written endpoint docs do.

## Features
 
- **Role-based access** — Admin, Landlord, and Tenant roles via ASP.NET Identity + JWT bearer auth
- **Property listings** — CRUD, image upload (Cloudinary), amenities, availability
- **Booking lifecycle** — request → `AwaitingPayment` → `Confirmed` / `Rejected` / `Cancelled`, with role-scoped cancellation rules for tenants vs. landlords
- **Background jobs** — booking expiry, scheduled email notifications via Hangfire
- **Transactional email** — eight booking-lifecycle HTML email templates (confirmation, rejection, cancellation, expiry, etc.) sent via Mailjet
- **Race-safe concurrency** — compare-and-swap status transitions instead of pessimistic locking, with explicit lost-race detection

## Tech Stack
 
| Layer | Technology |
|---|---|
| Language / Runtime | C# / .NET 8 |
| API style | REST, CQRS via MediatR |
| Database | PostgreSQL (database-first via EF Core) |
| Background jobs | Hangfire |
| Email | Mailjet |
| Image storage | Cloudinary |
| Auth | ASP.NET Identity + JWT |
| Logging | Serilog |
| API docs | Swagger / OpenAPI |

The solution follows Clean Architecture, with dependencies pointing inward:
 
```
letiahomes.API            → Controllers, middleware, composition root
letiahomes.Application     → CQRS commands/queries, MediatR handlers, DTOs, interfaces
letiahomes.Domain          → Entities, enums, domain logic (no external dependencies)
letiahomes.Infrastructure  → EF Core DbContext, repositories, external service implementations
                              (Cloudinary, Mailjet, Hangfire jobs)


## Getting Started
 
### Prerequisites
 
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL 17](https://www.postgresql.org/download/)
- A [Cloudinary](https://cloudinary.com/) account (for image upload)
- A [Mailjet](https://www.mailjet.com/) account (for transactional email)

###Setup

 
1. **Clone the repo**
```bash
   git clone <YOUR_REPO_URL>
   cd letiahomes
```
 
2. **Configure secrets**
   Add the following to `letiahomes.API/appsettings.Development.json` or via [.NET user-secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets):
```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=letiahomes;Username=postgres;Password=yourpassword"
     },
     "Jwt": {
       "Key": "your-secret-key",
       "Issuer": "letiahomes",
       "Audience": "letiahomes-client",
       "ExpiryMinutes": 60
     },
     "Cloudinary": {
       "CloudName": "...",
       "ApiKey": "...",
       "ApiSecret": "..."
     },
     "Mailjet": {
       "ApiKey": "...",
       "ApiSecret": "...",
       "SenderEmail": "..."
     }
   }
```
 
3. **Apply migrations**
```bash
   dotnet ef database update -p letiahomes.Infrastructure -s letiahomes.API
```
 
4. **Run the API**
```bash
   dotnet run --project letiahomes.API
```
 
5. **Open Swagger**
   Navigate to `https://localhost:7072/swagger` and authenticate using the seeded admin account (see migration seed data) to explore protected endpoints.

   ### Default seeded user

    
| Role | Email | Password |
|---|---|---|
| Admin | `gbolahanagbeleye@gmail.com` | `Admin@123!` |
 
> Change this password before deploying anywhere beyond local development.
 
## Project Status
 
This project is under active development and also the documentation and design and tradeoffs file isn't available for now .

## License
This project is licensed under the MIT License — see [LICENSE](LICENSE) for details.
 
## Author
 
**Inioluwa**
>>>>>>> ecdc55eb3f6b396a478665a409b09b687bdbac7e
