namespace letiahomes.Application.DTOs.Auth
{
    public sealed class VerifyOtpRequest
    {
        public required string Email { get; init; }
        public required string OtpCode { get; init; }
    }
}