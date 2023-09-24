using Telegram.Bot.Types;

namespace TelegramBots.BotAlarm.Domain.Model.Entities;

public class AlarmLogMessage
{
    public AlarmLogMessage(Message message, DbChat dbChat)
    {
        this.MessageId = message.MessageId;
        this.MessageText = message.Text!;
        this.Chat = dbChat;
        this.DateTime = DateTime.UtcNow;
    }

    private AlarmLogMessage()
    {
    }

    public int Id { get; set; }

    public bool IsDeleted { get; set; }

    public int MessageId { get; set; }

    public string? MessageText { get; set; }

    public int AlarmLogId { get; set; }

    public AlarmLog? AlarmLog { get; set; }

    public int ChatId { get; set; }

    public DbChat? Chat { get; set; }

    public DateTime DateTime { get; set; }
}