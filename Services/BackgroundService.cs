using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

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
            var nextRun = DateTime.Today.AddHours(9);

            if (now > nextRun)
                nextRun = nextRun.AddDays(1);

            await Task.Delay(nextRun - now, stoppingToken);

            using var scope = _serviceProvider.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

            await emailService.SendDailyEmailsAsync();
        }
    }
}
