using Microsoft.EntityFrameworkCore;

namespace TelegramBots.BotAlarm.Application;

public class AlarmService : IAlarmService
{
    private readonly ISafeTelegramClient safeTelegramClient;
    private readonly AlarmBotContext alarmBotContext;

    public AlarmService(ISafeTelegramClient safeTelegramClient, AlarmBotContext alarmBotContext)
    {
        this.safeTelegramClient = safeTelegramClient;
        this.alarmBotContext = alarmBotContext;
    }

    public async Task RemoveAlarmLogAsync(int alarmLogId)
    {
        var alarmLog = await alarmBotContext.AlarmLogs
            .Include(alarmLog => alarmLog.AlarmLogMessages)
            .ThenInclude(dbAlarmLogMessage => dbAlarmLogMessage.Chat)
            .FirstAsync(alarmLog => alarmLog.Id == alarmLogId).ConfigureAwait(false);

        if (!alarmLog.IsDeleted)
        {
            foreach (var alarmLogMessage in alarmLog.AlarmLogMessages)
            {
                await safeTelegramClient.DeleteTelegramMessage(alarmLogMessage.Chat!.TelegramId, alarmLogMessage.MessageId).ConfigureAwait(false);
            }

            alarmLog.MarkAsDeleted();
            await alarmBotContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}