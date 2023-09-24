using Telegram.Bot.Types;

namespace TelegramBots.BotAlarm.Domain.Model.Entities;

public class CurfewLogMessage
{
    public CurfewLogMessage(Message message, DbChat dbChat)
    {
        this.MessageId = message.MessageId;
        this.MessageText = message.Text!;
        this.Chat = dbChat;
        this.DateTime = DateTime.UtcNow;
    }

    private CurfewLogMessage()
    {
    }

    public int Id { get; set; }

    public bool IsDeleted { get; set; }

    public int MessageId { get; set; }

    public string? MessageText { get; set; }

    public int CurfewLogId { get; set; }

    public CurfewLog? CurfewLog { get; set; }

    public int ChatId { get; set; }

    public DbChat? Chat { get; set; }

    public DateTime DateTime { get; set; }
}