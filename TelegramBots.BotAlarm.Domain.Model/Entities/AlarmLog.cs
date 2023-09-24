using Telegram.Bot.Types;

using TelegramBots.BotAlarm.Domain.Model.Enums;

namespace TelegramBots.BotAlarm.Domain.Model.Entities;

public class AlarmLog
{
    public AlarmLog(AlarmEventType eventType)
    {
        this.EventType = eventType;
        this.DateTime = DateTime.UtcNow;
    }

    private AlarmLog()
    {
    }

    public int Id { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTime DateTime { get; set; }

    public AlarmEventType EventType { get; private set; }

    public ICollection<AlarmLogMessage> AlarmLogMessages { get; } = new List<AlarmLogMessage>();

    public void AddMessage(Message? message, DbChat dbChat)
    {
        if (message != null)
        {
            var alarmLogMessage = new AlarmLogMessage(message, dbChat);
            this.AlarmLogMessages.Add(alarmLogMessage);
        }
    }

    public void MarkAsDeleted()
    {
        foreach (var alarmLogMessage in this.AlarmLogMessages)
        {
            alarmLogMessage.IsDeleted = true;
        }
    }
}