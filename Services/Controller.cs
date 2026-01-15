using Microsoft.AspNetCore.Mvc;
using WebApplication5.Services;

namespace WebApplication5.Services
{


    [ApiController]
    [Route("api/daily-mail")]
    public class DailyMailController : ControllerBase
    {
        private readonly EmailService _emailService;

        public DailyMailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> Send(
     [FromHeader(Name = "X-CRON-SECRET")] string secret,
     IConfiguration config)
        {
            if (secret != config["DailyMail:Secret"])
                return Unauthorized();

            await _emailService.SendDailyEmailsAsync();
            return Ok();
        }

    }
}

