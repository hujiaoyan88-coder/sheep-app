using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace WebApplication5.Services
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly string _apiKey;

        public SendGridEmailSender(string apiKey) 
        {
            _apiKey = apiKey;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                return;
            }

            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("hujiaoyan88@gmail.com", "メリーさん");
            var to = new EmailAddress(email);

            var msg = MailHelper.CreateSingleEmail(
                from,
                to,
                subject,
                null,
                htmlMessage
            );

            await client.SendEmailAsync(msg);
        }
    }
}
