using TelegramBots.BotAlarm.Domain.Base;

namespace TelegramBots.BotAlarm.Presentation;

public class Scheduler : IHostedService, IDisposable
{
    private readonly IServiceProvider serviceProvider;

    private Timer? timer;

    public Scheduler(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        this.timer = new Timer(
            _ =>
            {
                var scope = this.serviceProvider.CreateScope();
                var alarmStatusCheckingService = scope.ServiceProvider.GetRequiredService<IAlarmStatusCheckingService>();
                alarmStatusCheckingService.CheckForAlarmAndNotifyAsync();

                var scope2 = this.serviceProvider.CreateScope();
                var curfewStatusCheckingService = scope2.ServiceProvider.GetRequiredService<ICurfewStatusCheckingService>();
                curfewStatusCheckingService.CheckForCurfewAndNotifyAsync(DateTime.UtcNow);
            },
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(15));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.timer?.Dispose();
        }
    }
}