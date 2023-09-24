using TelegramBots.BotAlarm.Domain.Model.Entities;

namespace TelegramBots.BotAlarm.Persistence.Base;

public interface ICurfewLogRepository
{
    Task AddLogAsync(CurfewLog curfewLog);

    Task<CurfewLog> GetLogAsync(int id);

    Task SaveChangesAsync();

}