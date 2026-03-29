namespace letiahomes.Application.Settings
{
    public sealed class MailjetSettings
    {
        public string ApiKey { get; init; } = string.Empty;
        public string ApiSecret { get; init; } = string.Empty;
        public string SenderEmail { get; init; } = string.Empty;
        public string SenderName { get; init; } = string.Empty;
    }
}