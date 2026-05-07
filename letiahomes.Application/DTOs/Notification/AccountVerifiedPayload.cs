namespace letiahomes.Application.DTOs.Notification
{
    public sealed record AccountVerifiedPayload(string Recipient,
    string FirstName,
    string LoginLink);
}
