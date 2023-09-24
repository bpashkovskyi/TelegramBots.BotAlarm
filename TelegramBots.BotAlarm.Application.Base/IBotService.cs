namespace TelegramBots.BotAlarm.Application.Base;

public interface IBotService
{
    Task StartAutomaticChecking();

    Task StopAutomaticChecking();
}