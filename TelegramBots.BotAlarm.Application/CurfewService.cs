using Microsoft.EntityFrameworkCore;

namespace TelegramBots.BotAlarm.Application;

public class CurfewService : ICurfewService
{
    private readonly ISafeTelegramClient safeTelegramClient;
    private readonly AlarmBotContext alarmBotContext;

    public CurfewService(ISafeTelegramClient safeTelegramClient, AlarmBotContext alarmBotContext)
    {
        this.safeTelegramClient = safeTelegramClient;
        this.alarmBotContext = alarmBotContext;
    }

    public async Task RemoveCurfewLogAsync(int curfewLogId)
    {
        var curfewLog = await alarmBotContext.CurfewLogs
            .Include(curfewLog => curfewLog.CurfewLogMessages)
            .ThenInclude(dbCurfewLogMessage => dbCurfewLogMessage.Chat)
            .FirstAsync(curfewLog => curfewLog.Id == curfewLogId).ConfigureAwait(false);

        if (!curfewLog.IsDeleted)
        {
            foreach (var curfewLogMessage in curfewLog.CurfewLogMessages)
            {
                await safeTelegramClient.DeleteTelegramMessage(curfewLogMessage.Chat!.TelegramId, curfewLogMessage.MessageId).ConfigureAwait(false);
            }
        }

        curfewLog.MarkAsDeleted();
        await alarmBotContext.SaveChangesAsync().ConfigureAwait(false);
    }
}