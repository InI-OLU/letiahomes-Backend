using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.Common.Exceptions;
using letiahomes.Application.Settings;
using Mailjet.Client;
using Mailjet.Client.Exceptions;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
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

        private const string DateFormat = "ddd, dd MMM yyyy";
        private static string FormatNaira(long kobo) => (kobo / 100m).ToString("N0");

        public Task SendAccountVerificationAsync(string recipient, string firstName, string link)
            => SendTemplatedEmailAsync(new EmailTemplateRequest(
                "AccountVerification.html",
                new Dictionary<string, string>
                {
                    ["FirstName"] = firstName,
                    ["LINK"] = link,
                },
                recipient,
                "Verify your account",
                 $"Hello {firstName}, Click on this link to verify your account : {link}"));

        public Task SendAccountVerifiedAsync(string recipient, string firstName, string loginLink)
       => SendTemplatedEmailAsync(new EmailTemplateRequest(
           "AccountVerified.html",
           new Dictionary<string, string>
           {
               ["FirstName"] = firstName,
               ["LOGIN_LINK"] = loginLink
           },
           recipient,
           "Your Letia Homes Account is Verified",
           $"Hello {firstName}, your account has been verified. Login here: {loginLink}"));

        public Task SendPasswordResetAsync(string recipient, string firstName, string resetLink)
          => SendTemplatedEmailAsync(new EmailTemplateRequest(
         "ResetPassword.html",
         new Dictionary<string, string>
         {
             ["FirstName"] = firstName,
             ["RESET_LINK"] = resetLink
         },
         recipient,
         "Password Reset Request",
         $"Hello {firstName}, you can reset your password using this link: {resetLink}"));

        public Task SendBookingRequestedLandlordEmailAsync(BookingRequestedLandlordPayload payload)
          => SendTemplatedEmailAsync(new EmailTemplateRequest(
              "BookingRequestedLandlord.html",
              new Dictionary<string, string>
              {
                  ["LandlordFirstName"] = payload.LandlordFirstName,
                  ["TenantFirstName"] = payload.TenantFirstName,
                  ["PropertyTitle"] = payload.PropertyTitle,
                  ["CheckIn"] = payload.CheckIn.ToString(DateFormat),
                  ["CheckOut"] = payload.CheckOut.ToString(DateFormat),
                  ["DashboardLink"] = payload.DashboardLink
              },
              payload.Recipient,
              "New Booking Request — Action Required",
              $"Hello {payload.LandlordFirstName}, {payload.TenantFirstName} has requested to book " +
              $"{payload.PropertyTitle} from {payload.CheckIn:d} to {payload.CheckOut:d}. " +
              $"Please respond within 24 hours: {payload.DashboardLink}"));


        public Task SendBookingRequestedTenantEmailAsync(BookingRequestedTenantPayload payload)
      => SendTemplatedEmailAsync(new EmailTemplateRequest(
          "BookingRequestedTenant.html",
          new Dictionary<string, string>
          {
              ["TenantFirstName"] = payload.TenantFirstName,
              ["PropertyTitle"] = payload.PropertyTitle,
              ["CheckIn"] = payload.CheckIn.ToString(DateFormat),
              ["CheckOut"] = payload.CheckOut.ToString(DateFormat),
              ["TotalAmountNaira"] = FormatNaira(payload.TotalAmountKobo)
          },
          payload.Recipient,
          "Booking Request Received",
          $"Hello {payload.TenantFirstName}, your request for {payload.PropertyTitle} has been " +
          $"sent to the landlord. You'll hear back within 24 hours."));

        public Task SendBookingConfirmedTenantEmailAsync(BookingConfirmedTenantPayload payload)
           => SendTemplatedEmailAsync(new EmailTemplateRequest(
               "BookingConfirmedTenant.html",
               new Dictionary<string, string>
               {
                   ["TenantFirstName"] = payload.TenantFirstName,
                   ["PropertyTitle"] = payload.PropertyTitle,
                   ["CheckIn"] = payload.CheckIn.ToString(DateFormat),
                   ["CheckOut"] = payload.CheckOut.ToString(DateFormat),
                   ["TotalAmountNaira"] = FormatNaira(payload.TotalAmountKobo),
                   ["PaymentLink"] = payload.PaymentLink,
                   ["PaymentExpiresAt"] = payload.PaymentExpiresAt.ToString("h:mm tt 'on' ddd, dd MMM")
               },
               payload.Recipient,
               "Your Booking Is Confirmed — Complete Payment",
               $"Hello {payload.TenantFirstName}, your booking for {payload.PropertyTitle} is confirmed. " +
               $"Complete payment within 2 hours to secure it: {payload.PaymentLink}"));

        public Task SendBookingConfirmedLandlordEmailAsync(BookingConfirmedLandlordPayload payload)
      => SendTemplatedEmailAsync(new EmailTemplateRequest(
          "BookingConfirmedLandlord.html",
          new Dictionary<string, string>
          {
              ["LandlordFirstName"] = payload.LandlordFirstName,
              ["TenantFirstName"] = payload.TenantFirstName,
              ["PropertyTitle"] = payload.PropertyTitle,
              ["CheckIn"] = payload.CheckIn.ToString(DateFormat),
              ["CheckOut"] = payload.CheckOut.ToString(DateFormat)
          },
          payload.Recipient,
          "You Confirmed a Booking",
          $"Hello {payload.LandlordFirstName}, you confirmed {payload.TenantFirstName}'s booking " +
          $"for {payload.PropertyTitle}. We'll notify you once payment is received."));

        public Task SendBookingRejectedEmailAsync(BookingRejectedPayload payload)
        => SendTemplatedEmailAsync(new EmailTemplateRequest(
            "BookingRejected.html",
            new Dictionary<string, string>
            {
                ["TenantFirstName"] = payload.TenantFirstName,
                ["PropertyTitle"] = payload.PropertyTitle,
                ["CheckIn"] = payload.CheckIn.ToString(DateFormat),
                ["CheckOut"] = payload.CheckOut.ToString(DateFormat),
                ["Reason"] = string.IsNullOrWhiteSpace(payload.Reason)
                    ? "No reason provided"
                    : payload.Reason,
                ["BrowseLink"] = payload.BrowseLink
            },
            payload.Recipient,
            "Update on Your Booking Request",
            $"Hello {payload.TenantFirstName}, unfortunately your request for {payload.PropertyTitle} " +
            $"was not accepted. No payment was taken."));

        public Task SendBookingCancelledEmailAsync(BookingCancelledPayload payload)
        {
            var summary = BuildCancellationSummary(payload);
            var refundLine = BuildRefundLine(payload);

            return SendTemplatedEmailAsync(new EmailTemplateRequest(
                "BookingCancelled.html",
                new Dictionary<string, string>
                {
                    ["RecipientFirstName"] = payload.RecipientFirstName,
                    ["PropertyTitle"] = payload.PropertyTitle,
                    ["CheckIn"] = payload.CheckIn.ToString(DateFormat),
                    ["CheckOut"] = payload.CheckOut.ToString(DateFormat),
                    ["Reason"] = string.IsNullOrWhiteSpace(payload.Reason)
                        ? "Not specified"
                        : payload.Reason,
                    ["CancellationSummary"] = summary,
                    ["RefundLine"] = refundLine
                },
                payload.Recipient,
                "Your Booking Has Been Cancelled",
                $"Hello {payload.RecipientFirstName}, {summary}"));
        }

        public Task SendBookingCompletedTenantEmailAsync(BookingCompletedTenantPayload payload)
    => SendTemplatedEmailAsync(new EmailTemplateRequest(
        "BookingCompletedTenant.html",
        new Dictionary<string, string>
        {
            ["TenantFirstName"] = payload.TenantFirstName,
            ["PropertyTitle"] = payload.PropertyTitle,
            ["CheckOut"] = payload.CheckOut.ToString(DateFormat),
            ["ReviewLink"] = payload.ReviewLink
        },
        payload.Recipient,
        "Your Stay Is Complete — Leave a Review",
        $"Hello {payload.TenantFirstName}, your stay at {payload.PropertyTitle} has ended. " +
        $"Leave a review here: {payload.ReviewLink}"));


        public Task SendBookingCompletedLandlordEmailAsync(BookingCompletedLandlordPayload payload)
         => SendTemplatedEmailAsync(new EmailTemplateRequest(
             "BookingCompletedLandlord.html",
             new Dictionary<string, string>
             {
                 ["LandlordFirstName"] = payload.LandlordFirstName,
                 ["PropertyTitle"] = payload.PropertyTitle,
                 ["CheckOut"] = payload.CheckOut.ToString(DateFormat),
                 ["PayoutAmountNaira"] = FormatNaira(payload.PayoutAmountKobo)
             },
             payload.Recipient,
             "Your Payout Is Being Processed",
             $"Hello {payload.LandlordFirstName}, the stay at {payload.PropertyTitle} has ended " +
             $"and your payout of ₦{FormatNaira(payload.PayoutAmountKobo)} is being processed."));

        // Builds the one variable sentence in the cancellation email.
        // Kept as a small, readable switch rather than nested ternaries.
        private static string BuildCancellationSummary(BookingCancelledPayload payload)
        {
            return payload.CancelledBy switch
            {
                "Tenant" when payload.IsRecipientTenant =>
                    $"You have cancelled your booking for {payload.PropertyTitle}.",

                "Tenant" =>
                    $"The tenant has cancelled their booking for {payload.PropertyTitle}. Your dates are now available again.",

                "Landlord" when payload.IsRecipientTenant =>
                    $"The landlord has cancelled your booking for {payload.PropertyTitle}. We apologize for the inconvenience — a full refund has been issued.",

                "Landlord" =>
                    $"You have cancelled this booking for {payload.PropertyTitle}.",

                "System" when payload.IsRecipientTenant =>
                    $"Your booking for {payload.PropertyTitle} was automatically cancelled because the payment or response window expired.",

                "System" =>
                    $"This booking for {payload.PropertyTitle} was automatically cancelled because the tenant did not complete the required action in time. Your dates are now available again.",

                _ => $"Your booking for {payload.PropertyTitle} has been cancelled."
            };
        }

        private static string BuildRefundLine(BookingCancelledPayload payload)
        {
            if (!payload.IsRecipientTenant || payload.RefundAmountKobo <= 0)
                return string.Empty;

            return $"A refund of ₦{FormatNaira(payload.RefundAmountKobo)} has been initiated and will reflect in your account shortly.";
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

        private sealed record EmailTemplateRequest (
            string TemplateFileName,
            Dictionary<string,string> Placeholders,
            string Recipient,
            string Subject,
            string PlainTextFallback);
        private async Task SendTemplatedEmailAsync (EmailTemplateRequest request) 
        {
            try
            {
                var templatePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot", "EmailTemplate", request.TemplateFileName
            );

                var template = await File.ReadAllTextAsync(templatePath);
                var htmlBody = request.Placeholders.Aggregate(
                  template,
                  (current, placeholder) =>
                      current.Replace("{{" + placeholder.Key + "}}", placeholder.Value));
                var mailjetRequest = new MailjetRequest
                {
                    Resource = SendV31.Resource
                }.Property("Messages", new JArray
                {
                    new JObject
                    {
                        {"From", new JObject
                        {
                            {"Email", _jetSettings.SenderEmail },
                            {"Name", _jetSettings.SenderName }
                        }
                      },
                { "To", new JArray { new JObject { { "Email", request.Recipient } } } },
                { "Subject", request.Subject },
                { "TextPart", request.PlainTextFallback },
                { "HtmlPart", htmlBody }
                    }
                });
                var response = await _mailjetClient.PostAsync(mailjetRequest);
                if (!response.IsSuccessStatusCode)
                    ClassifyAndThrow(response, request.Recipient);
            }
            catch (PermanentException) { throw; }
            catch (TemporaryException) { throw; }
            catch (MailjetException ex)
            {
                throw new TemporaryException("Mailjet SDK error", ex);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,
                     "An error occurred while sending a templated email to {Recipient}",
                     request.Recipient);
                throw new TemporaryException("Unexpected email error", ex);

            }

        }
    }
}