public sealed record BookingConfirmedLandlordPayload(
       string Recipient,
       string LandlordFirstName,
       string TenantFirstName,
       string PropertyTitle,
       DateTime CheckIn,
       DateTime CheckOut);