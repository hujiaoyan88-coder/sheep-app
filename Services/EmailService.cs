using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

public class EmailService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmailSender _emailSender;

    public EmailService(
        UserManager<IdentityUser> userManager,
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _emailSender = emailSender;
    }

    public async Task SendDailyEmailsAsync()
    {
        var users = await _userManager.Users
            .Where(u => u.EmailConfirmed)
            .ToListAsync();

        foreach (var user in users)
        {
            await _emailSender.SendEmailAsync(
                user.Email,
                "羊を探してください 🐏",
                """
                <p>羊を探してください 🐏</p>
                <p>
                  <a href="https://sheep-app.onrender.com/Identity/Account/Login">
                    ログイン
                  </a>
                </p>
                """
            );
        }
    }
}
