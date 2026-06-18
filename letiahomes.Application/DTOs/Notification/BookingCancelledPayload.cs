public sealed record BookingCancelledPayload(
        string Recipient,
        string RecipientFirstName,
        string PropertyTitle,
        DateTime CheckIn,
        DateTime CheckOut,
        string CancelledBy,        // "Tenant" | "Landlord" | "System"
        string? Reason,
        long RefundAmountKobo,     // 0 if no refund applies
        bool IsRecipientTenant);   // controls wording — tenant vs landlord framing