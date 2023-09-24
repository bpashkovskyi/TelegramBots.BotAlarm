using Telegram.Bot.Types;

using TelegramBots.BotAlarm.Domain.Model.Enums;

namespace TelegramBots.BotAlarm.Domain.Model.Entities;

public class CurfewLog
{
    public CurfewLog(CurfewEventType eventType)
    {
        this.EventType = eventType;
        this.DateTime = DateTime.UtcNow;
    }

    private CurfewLog()
    {
    }

    public int Id { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTime DateTime { get; private set; }

    public CurfewEventType EventType { get; private set; }

    public ICollection<CurfewLogMessage> CurfewLogMessages { get; private set; } = new List<CurfewLogMessage>();

    public void AddMessage(Message? message, DbChat dbChat)
    {
        if (message != null)
        {
            var curfewLogMessage = new CurfewLogMessage(message, dbChat);
            this.CurfewLogMessages.Add(curfewLogMessage);
        }
    }

    public void MarkAsDeleted()
    {
        foreach (var curfewLogMessage in this.CurfewLogMessages)
        {
            curfewLogMessage.IsDeleted = true;
        }
    }
}