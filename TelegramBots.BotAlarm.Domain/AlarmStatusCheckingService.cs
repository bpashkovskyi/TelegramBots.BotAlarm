namespace TelegramBots.BotAlarm.Domain;

public class AlarmStatusCheckingService : IAlarmStatusCheckingService
{
    private readonly ILogger rollbar;

    private readonly IAlarmService _alarmService;
    private readonly IAlarmApiClient alarmApiClient;
    private readonly AlarmBotContext alarmBotContext;

    public AlarmStatusCheckingService(
        IRollbar rollbar,
        IAlarmService alarmService,
        IAlarmApiClient alarmApiClient,
        AlarmBotContext alarmBotContext)
    {
        this.rollbar = rollbar;

        this._alarmService = alarmService;
        this.alarmApiClient = alarmApiClient;
        this.alarmBotContext = alarmBotContext;
    }

    public async Task CheckForAlarmAndNotifyAsync()
    {
        try
        {
            var lastServiceLog = await this.alarmBotContext.ServiceLogs.Last();
            if (lastServiceLog?.ServiceType == ServiceType.Stop)
            {
                return;
            }

            var regionAlarms = await this.alarmApiClient.GetRegionAlarmsAsync();
            if (regionAlarms == null)
            {
                return;
            }

            var ifRegion = regionAlarms.First(region => region.Key == AppSettings.IfRegion);
            var isAlarmActive = ifRegion.Value;

            var lastAlarmLog = await this.alarmBotContext.AlarmLogs.NotDeleted().Last();

            if (this.IsAlarmHasToBeNotified(lastAlarmLog, isAlarmActive))
            {
                await this._alarmService.NotifyAlarmAsync();
            }
            else if (this.IsContinuationHasToBeNotified(lastAlarmLog, isAlarmActive))
            {
                await this._alarmService.NotifyContinuationAsync();
            }
            else if (this.IsRejectHasToBeNotified(lastAlarmLog, isAlarmActive))
            {
                await this._alarmService.NotifyRejectAsync();
            }
            else if (this.IsQuietTimeHasToBeNotified(lastAlarmLog, isAlarmActive))
            {
                var lastAlarmLogWithTypeReject = await this.alarmBotContext.AlarmLogs.WithEventType(AlarmEventType.Reject).NotDeleted().Last();
                if (lastAlarmLogWithTypeReject != null)
                {
                    var hoursWithoutAlarm = (int)(DateTime.UtcNow - lastAlarmLogWithTypeReject.DateTime).TotalHours;
                    await this._alarmService.NotifyQuiteTimeAsync(hoursWithoutAlarm);
                }
            }
        }
        catch (Exception exception)
        {
            this.rollbar.Critical(exception);
        }
    }

    private bool IsAlarmHasToBeNotified(AlarmLog? lastAlarmLog, bool isAlarmActive)
    {
        if (isAlarmActive)
        {
            if (lastAlarmLog == null || (lastAlarmLog.EventType != AlarmEventType.Alarm && lastAlarmLog.EventType != AlarmEventType.Continue))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsRejectHasToBeNotified(AlarmLog? lastAlarmLog, bool isAlarmActive)
    {
        if (!isAlarmActive)
        {
            if (lastAlarmLog is { EventType: AlarmEventType.Alarm or AlarmEventType.Continue })
            {
                return true;
            }
        }

        return false;
    }

    private bool IsContinuationHasToBeNotified(AlarmLog? lastAlarmLog, bool isAlarmActive)
    {
        if (isAlarmActive && lastAlarmLog != null)
        {
            var hoursSpentFromLastNotification = (DateTime.UtcNow - lastAlarmLog.DateTime).TotalHours;
            if (hoursSpentFromLastNotification >= 1)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsQuietTimeHasToBeNotified(AlarmLog? lastAlarmLog, bool isAlarmActive)
    {
        if (!isAlarmActive && lastAlarmLog != null && lastAlarmLog.EventType != AlarmEventType.Alarm && lastAlarmLog.EventType != AlarmEventType.Continue)
        {
            var hoursSpentFromLastNotification = (DateTime.UtcNow - lastAlarmLog.DateTime).TotalHours;
            if (hoursSpentFromLastNotification >= 1)
            {
                return true;
            }
        }

        return false;
    }
}