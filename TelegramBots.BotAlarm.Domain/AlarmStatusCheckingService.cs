namespace TelegramBots.BotAlarm.Domain;

public class AlarmStatusCheckingService : IAlarmStatusCheckingService
{
    private readonly ILogger rollbar;

    private readonly IAlarmNotificationService alarmNotificationService;
    private readonly IAlarmApiClient alarmApiClient;
    private readonly AlarmBotContext alarmBotContext;

    public AlarmStatusCheckingService(
        IRollbar rollbar,
        IAlarmNotificationService alarmNotificationService,
        IAlarmApiClient alarmApiClient,
        AlarmBotContext alarmBotContext)
    {
        this.rollbar = rollbar;

        this.alarmNotificationService = alarmNotificationService;
        this.alarmApiClient = alarmApiClient;
        this.alarmBotContext = alarmBotContext;
    }

    public async Task CheckForAlarmAndNotifyAsync()
    {
        try
        {
            var lastServiceLog = await this.alarmBotContext.ServiceLogs.Last().ConfigureAwait(false);
            if (lastServiceLog?.ServiceType == ServiceType.Stop)
            {
                return;
            }

            var regionAlarms = await this.alarmApiClient.GetRegionAlarmsAsync().ConfigureAwait(false);
            if (regionAlarms == null)
            {
                return;
            }

            var ifRegion = regionAlarms.First(region => region.Key == AppSettings.IfRegion);
            var isAlarmActive = ifRegion.Value;

            var lastAlarmLog = await this.alarmBotContext.AlarmLogs.NotDeleted().Last().ConfigureAwait(false);

            if (this.IsAlarmHasToBeNotified(lastAlarmLog, isAlarmActive))
            {
                await this.alarmNotificationService.NotifyAlarmAsync().ConfigureAwait(false);
            }
            else if (this.IsContinuationHasToBeNotified(lastAlarmLog, isAlarmActive))
            {
                await this.alarmNotificationService.NotifyContinuationAsync().ConfigureAwait(false);
            }
            else if (this.IsRejectHasToBeNotified(lastAlarmLog, isAlarmActive))
            {
                await this.alarmNotificationService.NotifyRejectAsync().ConfigureAwait(false);
            }
            else if (this.IsQuietTimeHasToBeNotified(lastAlarmLog, isAlarmActive))
            {
                var lastAlarmLogWithTypeReject = await this.alarmBotContext.AlarmLogs.WithEventType(AlarmEventType.Reject).NotDeleted().Last().ConfigureAwait(false);
                if (lastAlarmLogWithTypeReject != null)
                {
                    var hoursWithoutAlarm = (int)(DateTime.UtcNow - lastAlarmLogWithTypeReject.DateTime).TotalHours;
                    await this.alarmNotificationService.NotifyQuiteTimeAsync(hoursWithoutAlarm).ConfigureAwait(false);
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