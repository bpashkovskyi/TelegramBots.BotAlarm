using TelegramBots.BotAlarm.Domain.Model.Entities;

namespace TelegramBots.BotAlarm.Persistence.Base
{
    public interface IAlarmLogRepository
    {
        Task AddLogAsync(AlarmLog alarmLog);

        Task<AlarmLog> GetLogAsync(int id);

        Task SaveChangesAsync();
    }
}
