namespace TelegramBots.BotAlarm.Domain;

public class CurfewStatusCheckingService : ICurfewStatusCheckingService
{
    private readonly ILogger rollbar;

    private readonly ICurfewService _curfewService;
    private readonly AlarmBotContext alarmBotContext;

    public CurfewStatusCheckingService(IRollbar rollbar, ICurfewService curfewService, AlarmBotContext alarmBotContext)
    {
        this.rollbar = rollbar;
        this._curfewService = curfewService;
        this.alarmBotContext = alarmBotContext;
    }

    public async Task CheckForCurfewAndNotifyAsync(DateTime dateTime)
    {
        try
        {
            var lastServiceLog = await this.alarmBotContext.ServiceLogs.Last();
            if (lastServiceLog?.ServiceType == ServiceType.Stop)
            {
                return;
            }

            var lastCurfewLog = await this.alarmBotContext.CurfewLogs.NotDeleted().Last();

            var kyivTimeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
            var kyivTime = TimeZoneInfo.ConvertTime(dateTime, kyivTimeZone);
            var isNight = kyivTime.Hour is >= 0 and < 5;

            if (this.IsNightHasToBeNotified(lastCurfewLog, isNight))
            {
                await this._curfewService.NotifyNightAsync();
            }
            else if (this.IsDayHasToBeNotified(lastCurfewLog, isNight))
            {
                await this._curfewService.NotifyDayAsync();
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