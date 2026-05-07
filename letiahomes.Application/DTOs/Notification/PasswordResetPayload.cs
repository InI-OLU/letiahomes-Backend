

namespace letiahomes.Application.DTOs.Notification
{
    public sealed record PasswordResetPayload(string Recipient, string FirstName, string ResetLink);
}
