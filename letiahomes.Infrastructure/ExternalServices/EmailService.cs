using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.Common.Exceptions;
using letiahomes.Application.Settings;
using Mailjet.Client;
using Mailjet.Client.Exceptions;
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

        public async Task SendAsync(string recipient, string subject, string message)
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
                if (!response.IsSuccessStatusCode)
                {
                    ClassifyAndThrow(response, recipient);
                }
            
            }
            catch (PermanentException)
            {
                throw;
            }
            catch (TemporaryException)
            {
                throw;
            }
            catch (MailjetException ex)
            {
                throw new TemporaryException("Mailjet SDK error", ex);
            }
        }

        public async Task SendAccountVerifiedAsync(string recipient, string firstName, string loginLink)
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
                if (!response.IsSuccessStatusCode)
                {
                    ClassifyAndThrow(response, recipient);
                }
             
            }
            catch (PermanentException)
            {
                throw;
            }
            catch (TemporaryException)
            {
                throw;
            }
            catch (MailjetException ex)
            {
                throw new TemporaryException("Mailjet SDK error", ex);
            }
        }

        public async Task SendPasswordResetAsync(string recipient, string firstName, string resetLink)
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
              if (!response.IsSuccessStatusCode)
                {
                     ClassifyAndThrow(response, recipient);
                }
               
               
            }
            catch (PermanentException)
            {
                throw; 
            }
            catch (TemporaryException)
            {
                throw; 
            }
            catch (MailjetException ex)
            {
                throw new TemporaryException("Mailjet SDK error", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Recipient}", recipient);
                throw new TemporaryException("Unexpected email error", ex);
            }
        }
        private void ClassifyAndThrow (MailjetResponse response, string recipient)
        {
            int StatusCode = response.StatusCode;
            string responseBody = response.GetData().ToString();
            switch (StatusCode)
            {
                //Permanent errors not to be retried
                case 400:
                    throw new PermanentException($"Bad  request to {recipient}");
                case 401:
                    throw new PermanentException("Mailjet authentication failed : check API keys");
                case 403:
                    throw new PermanentException(
                        "Mailjet account forbidden : check sender domain verification");
                case 404:
                    throw new PermanentException(
                        $"Mailjet resource not found: {responseBody}");

                //Transient errors to be retried 
                case 429:
                    throw new TemporaryException(
                        "Mailjet rate limit hit : retry after delay");
                case 500:
                case 502:
                case 503:
                case 504:
                    throw new TemporaryException(
                        $"Mailjet server error {StatusCode} — retry");

                default:
                    throw new TemporaryException(
                        $"Unknown Mailjet error {StatusCode}: {responseBody}");
            }
        }
    }
}