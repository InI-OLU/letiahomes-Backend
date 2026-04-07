using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.Settings;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace letiahomes.Infrastructure.ExternalServices
{
    public class EmailService : IEmailService
    {
        private readonly MailjetSettings _jetSettings;
        private readonly IMailjetClient _mailjetClient;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<MailjetSettings> jetOptions,
            IMailjetClient mailjetClient,
            ILogger<EmailService> logger)
        {
            _mailjetClient = mailjetClient;
            _logger = logger;
            _jetSettings = jetOptions.Value ??
                throw new ArgumentNullException(nameof(jetOptions), "MailJetSettings is not configured.");
        }

        public async Task<bool> SendAsync(string recipient, string subject, string message)
        {
            try
            {
                var request = new MailjetRequest
                {
                    Resource = SendV31.Resource
                }.Property("Messages", new JArray
                {
                    new JObject
                    {
                        {
                            "From", new JObject
                            {
                                { "Email", _jetSettings.SenderEmail },
                                { "Name", _jetSettings.SenderName }
                            }
                        },
                        {
                            "To", new JArray
                            {
                                new JObject
                                {
                                    { "Email", recipient }
                                }
                            }
                        },
                        { "Subject", subject },
                        { "TextPart", message },
                        { "HtmlPart", message }
                    }
                });

                var response = await _mailjetClient.PostAsync(request);
                return response?.IsSuccessStatusCode ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient} with subject '{Subject}'", recipient, subject);
                return false;
            }
        }

        public async Task<bool> SendAccountVerifiedAsync(string recipient, string firstName, string loginLink)
        {
            try
            {
                var templatePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot", "EmailTemplate", "AccountVerified.html"
                );

                var template = await File.ReadAllTextAsync(templatePath);

                var htmlBody = template
                    .Replace("{{FirstName}}", firstName)
                    .Replace("{{LOGIN_LINK}}", loginLink);

                var request = new MailjetRequest
                {
                    Resource = SendV31.Resource
                }.Property("Messages", new JArray
                {
                    new JObject
                    {
                        {
                            "From", new JObject
                            {
                                { "Email", _jetSettings.SenderEmail },
                                { "Name", _jetSettings.SenderName }
                            }
                        },
                        {
                            "To", new JArray
                            {
                                new JObject { { "Email", recipient } }
                            }
                        },
                        { "Subject", "Your Letia Homes Account is Verified" },
                        { "TextPart", $"Hello {firstName}, your account has been verified. Login here: {loginLink}" },
                        { "HtmlPart", htmlBody }
                    }
                });

                var response = await _mailjetClient.PostAsync(request);
                return response?.IsSuccessStatusCode ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send account verified email to {Recipient}", recipient);
                return false;
            }
        }

        public async Task<bool> SendPasswordResetAsync(string recipient, string firstName, string resetLink)
        {
            try
            {
                var templatePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot", "EmailTemplate", "ResetPassword.html"
                );

                var template = await File.ReadAllTextAsync(templatePath);

                var htmlBody = template
                    .Replace("{{FirstName}}", firstName)
                    .Replace("{{RESET_LINK}}", resetLink);

                var request = new MailjetRequest
                {
                    Resource = SendV31.Resource
                }.Property("Messages", new JArray
                {
                    new JObject
                    {
                        {
                            "From", new JObject
                            {
                                { "Email", _jetSettings.SenderEmail },
                                { "Name", _jetSettings.SenderName }
                            }
                        },
                        {
                            "To", new JArray
                            {
                                new JObject { { "Email", recipient } }
                            }
                        },
                        { "Subject", "Password Reset Request" },
                        { "TextPart", $"Hello {firstName}, you can reset your password using this link: {resetLink}" },
                        { "HtmlPart", htmlBody }
                    }
                });

                var response = await _mailjetClient.PostAsync(request);
                return response?.IsSuccessStatusCode ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Recipient}", recipient);
                return false;
            }
        }
    }
}