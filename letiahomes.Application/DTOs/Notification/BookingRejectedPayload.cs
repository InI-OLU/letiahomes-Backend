public sealed record BookingRejectedPayload(
       string Recipient,
       string TenantFirstName,
       string PropertyTitle,
       DateTime CheckIn,
       DateTime CheckOut,
       string? Reason,
       string BrowseLink);