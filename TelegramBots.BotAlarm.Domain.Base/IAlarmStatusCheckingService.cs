namespace TelegramBots.BotAlarm.Domain.Base;

public interface IAlarmStatusCheckingService
{
    Task CheckForAlarmAndNotifyAsync();
}