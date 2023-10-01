namespace TelegramBots.BotAlarm.Application.Base;

public interface IAlarmService
{
    Task NotifyAlarmAsync();

    Task NotifyContinuationAsync();

    Task NotifyRejectAsync();

    Task NotifyTestAsync();

    Task RemoveAlarmLogAsync(int alarmLogId);
}