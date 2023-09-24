namespace TelegramBots.BotAlarm.Application.Base;

public interface ICurfewService
{
    Task RemoveCurfewLogAsync(int curfewLogId);
}