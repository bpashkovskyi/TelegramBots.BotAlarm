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

    public async Task NotifyNightAsync()
    {
        var curfewLog = new CurfewLog(CurfewEventType.Night);
        var chats = await this.alarmBotContext.Chats.ToListAsync().ConfigureAwait(false);

        var chatsToBlockDuringCurfew = chats.Where(currentChat => currentChat.Settings.BlockChatDuringCurfew);
        foreach (var chatToBlockDuringCurfew in chatsToBlockDuringCurfew)
        {
            if (!chatToBlockDuringCurfew.Status.BlockedDuringAlarm)
            {
                await this.safeTelegramClient.BlockChatAsync(chatToBlockDuringCurfew.TelegramId).ConfigureAwait(false);

                ////var message = await this.safeTelegramClient.SendTextMessageAsync(chatToBlockDuringCurfew.TelegramId, AppSettings.CurfewBlockText).ConfigureAwait(false);
                ////curfewLog.AddMessage(message, chatToBlockDuringCurfew);
            }

            chatToBlockDuringCurfew.Status.BlockedDuringCurfew = true;
        }

        await this.alarmBotContext.AddAsync(curfewLog).ConfigureAwait(false);
        await this.alarmBotContext.SaveChangesAsync().ConfigureAwait(false);

        await this.safeTelegramClient.SendTextMessageAsync(AppSettings.AdminChatId, AppSettings.CurfewMessageSentText(curfewLog.Id)).ConfigureAwait(false);
    }

    public async Task NotifyDayAsync()
    {
        var curfewLog = new CurfewLog(CurfewEventType.Day);
        var chats = await this.alarmBotContext.Chats.ToListAsync().ConfigureAwait(false);

        var chatsToBlockDuringCurfew = chats.Where(currentChat => currentChat.Settings.BlockChatDuringCurfew);
        foreach (var chatToBlockDuringCurfew in chatsToBlockDuringCurfew)
        {
            if (!chatToBlockDuringCurfew.Status.BlockedDuringAlarm)
            {
                await this.safeTelegramClient.UnblockChatAsync(chatToBlockDuringCurfew.TelegramId).ConfigureAwait(false);

                ////var message = await this.safeTelegramClient.SendTextMessageAsync(chatToBlockDuringCurfew.TelegramId, AppSettings.CurfewUnblockText).ConfigureAwait(false);
                ////curfewLog.AddMessage(message, chatToBlockDuringCurfew);
            }

            chatToBlockDuringCurfew.Status.BlockedDuringCurfew = false;
        }

        await this.alarmBotContext.AddAsync(curfewLog).ConfigureAwait(false);
        await this.alarmBotContext.SaveChangesAsync().ConfigureAwait(false);

        await this.safeTelegramClient.SendTextMessageAsync(AppSettings.AdminChatId, AppSettings.CurfewMessageSentText(curfewLog.Id)).ConfigureAwait(false);
    }

    public async Task RemoveCurfewLogAsync(int curfewLogId)
    {
        var curfewLog = await this.alarmBotContext.CurfewLogs
            .Include(curfewLog => curfewLog.CurfewLogMessages)
            .ThenInclude(dbCurfewLogMessage => dbCurfewLogMessage.Chat)
            .FirstAsync(curfewLog => curfewLog.Id == curfewLogId).ConfigureAwait(false);

        if (!curfewLog.IsDeleted)
        {
            foreach (var curfewLogMessage in curfewLog.CurfewLogMessages)
            {
                await this.safeTelegramClient.DeleteTelegramMessage(curfewLogMessage.Chat!.TelegramId, curfewLogMessage.MessageId).ConfigureAwait(false);
            }
        }

        curfewLog.MarkAsDeleted();
        await this.alarmBotContext.SaveChangesAsync().ConfigureAwait(false);
    }
}