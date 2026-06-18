public sealed record BookingRequestedLandlordPayload(
     string Recipient,
     string LandlordFirstName,
     string TenantFirstName,
     string PropertyTitle,
     DateTime CheckIn,
     DateTime CheckOut,
     string DashboardLink);