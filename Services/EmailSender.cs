using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace WebApplication5.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
           


            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {   //Gmailにログインしてメールを送らせてください
                Credentials = new NetworkCredential(
                    "hujiaoyan88@gmail.com",
                    "uggv lhdy ybxm prng"
                ),
                EnableSsl = true
            };

            var message = new MailMessage
            {
                From = new MailAddress(
                    "hujiaoyan88@gmail.com",
                    "メリーさん"
                ),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            message.To.Add(email);


            await smtp.SendMailAsync(message);

        }
    }
}


