using TelegramBots.BotAlarm.Domain.Model.Enums;

namespace TelegramBots.BotAlarm.Domain.Model.Entities;

public class ServiceLog
{
    public ServiceLog(ServiceType serviceType)
    {
        this.ServiceType = serviceType;
        this.DateTime = DateTime.UtcNow;
    }

    private ServiceLog()
    {
    }

    public int Id { get; private set; }

    public DateTime DateTime { get; private set; }

    public ServiceType ServiceType { get; private set; }
}