public sealed record BookingCompletedLandlordPayload(
      string Recipient,
      string LandlordFirstName,
      string PropertyTitle,
      DateTime CheckOut,
      long PayoutAmountKobo);