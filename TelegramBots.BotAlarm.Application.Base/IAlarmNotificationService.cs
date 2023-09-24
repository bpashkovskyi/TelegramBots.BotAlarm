namespace TelegramBots.BotAlarm.Application.Base;

public interface IAlarmNotificationService
{
    Task NotifyAlarmAsync();

    Task NotifyContinuationAsync();

    Task NotifyRejectAsync();

    Task NotifyQuiteTimeAsync(int hoursWithoutAlarm);

    Task NotifyTestAsync();
}