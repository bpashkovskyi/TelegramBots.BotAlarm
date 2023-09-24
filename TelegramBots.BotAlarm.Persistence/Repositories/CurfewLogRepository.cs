using Microsoft.EntityFrameworkCore;

using TelegramBots.BotAlarm.Domain.Model.Entities;
using TelegramBots.BotAlarm.Persistence.Base;

namespace TelegramBots.BotAlarm.Persistence.Repositories;

public class CurfewLogRepository : ICurfewLogRepository
{
    private readonly AlarmBotContext alarmBotContext;

    public CurfewLogRepository(AlarmBotContext alarmBotContext)
    {
        this.alarmBotContext = alarmBotContext;
    }

    public async Task AddLogAsync(CurfewLog curfewLog)
    {
        await this.alarmBotContext.AddAsync(curfewLog);
    }

    public async Task<CurfewLog> GetLogAsync(int id)
    {
        return await this.alarmBotContext.CurfewLogs
            .Include(curfewLog => curfewLog.CurfewLogMessages)
            .ThenInclude(dbCurfewLogMessage => dbCurfewLogMessage.Chat)
            .FirstAsync(curfewLog => curfewLog.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await this.alarmBotContext.SaveChangesAsync();
    }
}