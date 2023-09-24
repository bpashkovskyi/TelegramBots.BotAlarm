namespace TelegramBots.BotAlarm.Application.Base;

public interface ICurfewNotificationService
{
    Task NotifyNightAsync();

    Task NotifyDayAsync();
}