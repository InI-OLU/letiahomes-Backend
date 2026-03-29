namespace letiahomes.Application.DTOs.Auth
{
    public abstract class RegisterBaseRequest
    {
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required string Email { get; init; }
        public required string Password { get; init; }
        public required string PhoneNumber { get; init; }
    }
}