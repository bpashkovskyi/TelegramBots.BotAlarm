namespace TelegramBots.BotAlarm.Domain.Model.ValueObjects;

public class ChatStatus
{
    public bool BlockedDuringAlarm { get; set; }

    public bool BlockedDuringCurfew { get; set; }
}