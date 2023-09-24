using Microsoft.EntityFrameworkCore;

using TelegramBots.BotAlarm.Domain.Model.Entities;
using TelegramBots.BotAlarm.Persistence.Base;

namespace TelegramBots.BotAlarm.Persistence.Repositories;

public class AlarmLogRepository : IAlarmLogRepository
{
    private readonly AlarmBotContext alarmBotContext;

    public AlarmLogRepository(AlarmBotContext alarmBotContext)
    {
        this.alarmBotContext = alarmBotContext;
    }

    public async Task AddLogAsync(AlarmLog alarmLog)
    {
        await this.alarmBotContext.AddAsync(alarmLog);
        await this.alarmBotContext.SaveChangesAsync();
    }

    public async Task<AlarmLog> GetLogAsync(int id)
    {
        return await this.alarmBotContext.AlarmLogs
            .Include(alarmLog => alarmLog.AlarmLogMessages)
            .ThenInclude(alarmLogMessage => alarmLogMessage.Chat)
            .FirstAsync(alarmLog => alarmLog.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await this.alarmBotContext.SaveChangesAsync();
    }
}