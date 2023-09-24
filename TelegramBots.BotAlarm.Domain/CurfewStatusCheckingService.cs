namespace TelegramBots.BotAlarm.Domain;

public class CurfewStatusCheckingService : ICurfewStatusCheckingService
{
    private readonly ILogger rollbar;

    private readonly ICurfewNotificationService curfewNotificationService;
    private readonly AlarmBotContext alarmBotContext;

    public CurfewStatusCheckingService(IRollbar rollbar, ICurfewNotificationService curfewNotificationService, AlarmBotContext alarmBotContext)
    {
        this.rollbar = rollbar;
        this.curfewNotificationService = curfewNotificationService;
        this.alarmBotContext = alarmBotContext;
    }

    public async Task CheckForCurfewAndNotifyAsync(DateTime dateTime)
    {
        try
        {
            var lastServiceLog = await this.alarmBotContext.ServiceLogs.Last().ConfigureAwait(false);
            if (lastServiceLog?.ServiceType == ServiceType.Stop)
            {
                return;
            }

            var lastCurfewLog = await this.alarmBotContext.CurfewLogs.NotDeleted().Last().ConfigureAwait(false);

            var kyivTimeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
            var kyivTime = TimeZoneInfo.ConvertTime(dateTime, kyivTimeZone);
            var isNight = kyivTime.Hour is >= 0 and < 5;

            if (this.IsNightHasToBeNotified(lastCurfewLog, isNight))
            {
                await this.curfewNotificationService.NotifyNightAsync().ConfigureAwait(false);
            }
            else if (this.IsDayHasToBeNotified(lastCurfewLog, isNight))
            {
                await this.curfewNotificationService.NotifyDayAsync().ConfigureAwait(false);
            }
        }
        catch (Exception exception)
        {
            this.rollbar.Critical(exception);
        }
    }

    private bool IsNightHasToBeNotified(CurfewLog? lastCurfewLog, bool isNight)
    {
        if (isNight)
        {
            if (lastCurfewLog is not { EventType: CurfewEventType.Night })
            {
                return true;
            }
        }

        return false;
    }

    private bool IsDayHasToBeNotified(CurfewLog? lastCurfewLog, bool isNight)
    {
        if (!isNight)
        {
            if (lastCurfewLog is not { EventType: CurfewEventType.Day })
            {
                return true;
            }
        }

        return false;
    }
}