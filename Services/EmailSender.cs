using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace WebApplication5.Services;

public class SendGridEmailSender : IEmailSender
{
    private readonly string _apiKey;

    public SendGridEmailSender(IOptions<SendGridOptions> options)
    {
        _apiKey = options.Value.ApiKey;
        Console.WriteLine("✅ SendGridEmailSender が生成された");

        if (string.IsNullOrEmpty(_apiKey))
            throw new InvalidOperationException("SendGrid API Key is missing");
    }
    

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        Console.WriteLine("📧 SendEmailAsync 開始");

        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            Console.WriteLine("❌ SENDGRID_API_KEY が空");
            return;
        }

        var client = new SendGridClient(_apiKey);

        var from = new EmailAddress("hujiaoyan88@gmail.com", "メリーさん");
        var to = new EmailAddress(email);

        var msg = MailHelper.CreateSingleEmail(
            from,
            to,
            subject,
            plainTextContent: null,
            htmlContent: htmlMessage
        );

        await client.SendEmailAsync(msg);
        Console.WriteLine("📧 SendEmailAsync 完了");
    }
}
