using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WebApplication5.Services // プロジェクトの名前空間に合わせる
{
    public class DailyEmailHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public DailyEmailHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var nextRunTime = DateTime.Today.AddHours(20); // 今日21時
                if (now > nextRunTime)
                    nextRunTime = nextRunTime.AddDays(1);

                var delay = nextRunTime - now;
                await Task.Delay(delay, stoppingToken);

                using var scope = _serviceProvider.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();
                await emailService.SendDailyEmailsAsync();
            }
        }
    }
}
