using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.Settings;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;

namespace letiahomes.Infrastructure.ExternalServices
{
    public class EmailService:IEmailService
    {

        private readonly MailjetSettings _jetSettings;
        private readonly IMailjetClient _mailjetClient;
        public EmailService(IOptions<MailjetSettings> jetOptions,
                            IMailjetClient mailjetClient)
        {
            _mailjetClient = mailjetClient;
            _jetSettings = jetOptions.Value ??
             throw new ArgumentNullException("MailJetSettings");
        }

        public async Task<bool> SendAsync(string recipient, string message, string subject)
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
                            "From",
                            new JObject
                            {
                                { "Email", _jetSettings.SenderEmail },
                                { "Name", _jetSettings.SenderName }
                            }
                        },
                        {
                            "To",
                            new JArray
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
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> SendAccountVerifiedAsync(string recipient, string firstName, string loginLink)
        {
            try
            {
                var templatePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot", "EmailTemplate", "account-verified.html"
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
            catch (Exception)
            {
                throw;
            }
        }
    }
}
