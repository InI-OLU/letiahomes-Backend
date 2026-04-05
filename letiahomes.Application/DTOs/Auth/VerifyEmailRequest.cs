namespace letiahomes.Application.DTOs.Auth
{
    public sealed class VerifyEmailRequest
    {
        public required Guid UserId { get; init; }
        public required string OtpCode { get; init; }
    }
}