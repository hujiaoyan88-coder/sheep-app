using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Security.Policy;
using System.Threading.Tasks;

namespace WebApplication5.Services
{
    public class EmailService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public EmailService(UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // 毎日21時に呼び出されるメソッド
        public async Task SendDailyEmailsAsync()
        {
            // ここでユーザー一覧を取得
            var users = _userManager.Users.ToList();

            foreach (var user in users)
            {

                if (!string.IsNullOrEmpty(user.Email))
                {
                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "羊を探してください",
                        "<p>羊を探してください🐏</p>" +
                        "<p><a>ログイン</a></p>"
                    );
                }
            }
        }
    }
}
