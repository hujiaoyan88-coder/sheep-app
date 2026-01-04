using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _config;
    public EmailSender(IConfiguration config) => _config = config;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var apiKey = _config["SendGrid:ApiKey"];
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress(_config["SendGrid:FromEmail"], "Sheep App Support");
        var to = new EmailAddress(email);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);

        await client.SendEmailAsync(msg);
    }
}