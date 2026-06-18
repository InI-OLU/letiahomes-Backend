public sealed record BookingRequestedTenantPayload(
      string Recipient,
      string TenantFirstName,
      string PropertyTitle,
      DateTime CheckIn,
      DateTime CheckOut,
      long TotalAmountKobo);