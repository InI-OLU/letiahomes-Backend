public sealed record BookingNotificationContext(
        Guid BookingId,
        string TenantEmail,
        string TenantFirstName,
        string LandlordEmail,
        string LandlordFirstName,
        string PropertyTitle,
        DateTime CheckIn,
        DateTime CheckOut,
        string? CancellationReason
    );
