namespace letiahomes.Application.DTOs.Auth
{
    public sealed class RegisterLandlordRequest : RegisterBaseRequest
    {
        public required string Address { get; init; }
        public required string GovernmentId { get; init; }

        
        public required string BankName { get; init; }
        public required string BankAccountNumber { get; init; }
        public required string BankAccountName { get; init; }
    }
}