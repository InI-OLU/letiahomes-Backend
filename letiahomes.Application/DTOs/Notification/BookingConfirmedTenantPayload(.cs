public sealed record BookingConfirmedTenantPayload(
     string Recipient,
     string TenantFirstName,
     string PropertyTitle,
     DateTime CheckIn,
     DateTime CheckOut,
     long TotalAmountKobo,
     string PaymentLink,
     DateTime PaymentExpiresAt);