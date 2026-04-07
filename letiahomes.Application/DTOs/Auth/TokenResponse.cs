namespace letiahomes.Application.DTOs.Auth
{
    public sealed record TokenResponse
    {
        public string AccessToken { get; init; }
        public string RefreshToken { get; init; }

        public TokenResponse(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}