namespace TelegramBots.BotAlarm.Domain.Base;

public interface ICurfewStatusCheckingService
{
    Task CheckForCurfewAndNotifyAsync(DateTime dateTime);
}