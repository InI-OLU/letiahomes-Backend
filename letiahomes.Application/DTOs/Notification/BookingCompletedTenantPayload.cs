public sealed record BookingCompletedTenantPayload(
      string Recipient,
      string TenantFirstName,
      string PropertyTitle,
      DateTime CheckOut);