namespace TelegramBots.BotAlarm.Application.Base;

public interface ICurfewService
{
    Task NotifyNightAsync();

    Task NotifyDayAsync();

    Task RemoveCurfewLogAsync(int curfewLogId);

}