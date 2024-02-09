namespace TelegramBots.BotAlarm.Domain.Model.ValueObjects;

public class ChatSettings
{
    public bool BroadcastMessageDuringAlarm { get; set; }

    public bool BlockChatDuringAlarm { get; set; }

    public bool BlockChatDuringCurfew { get; set; }
    
    public bool BroadcastMessageDuringBroadcast { get; set; }
}