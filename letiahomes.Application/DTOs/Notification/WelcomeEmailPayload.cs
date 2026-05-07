namespace letiahomes.Application.DTOs.Notification
{
    public sealed record WelcomeEmailPayload(
    string Recipient,
    string Subject,
    string Message);
}
