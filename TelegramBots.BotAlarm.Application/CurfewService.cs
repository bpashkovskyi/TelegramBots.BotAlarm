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
        var chats = await this.alarmBotContext.Chats.ToListAsync();

        var chatsToBlockDuringCurfew = chats.Where(currentChat => currentChat.Settings.BlockChatDuringCurfew);
        foreach (var chatToBlockDuringCurfew in chatsToBlockDuringCurfew)
        {
            if (!chatToBlockDuringCurfew.Status.BlockedDuringAlarm)
            {
                await this.safeTelegramClient.BlockChatAsync(chatToBlockDuringCurfew.TelegramId);

                ////var message = await this.safeTelegramClient.SendTextMessageAsync(chatToBlockDuringCurfew.TelegramId, AppSettings.CurfewBlockText);
                ////curfewLog.AddMessage(message, chatToBlockDuringCurfew);
            }

            chatToBlockDuringCurfew.Status.BlockedDuringCurfew = true;
        }

        await this.alarmBotContext.AddAsync(curfewLog);
        await this.alarmBotContext.SaveChangesAsync();

        await this.safeTelegramClient.SendTextMessageAsync(AppSettings.AdminChatId, AppSettings.CurfewMessageSentText(curfewLog.Id));
    }

    public async Task NotifyDayAsync()
    {
        var curfewLog = new CurfewLog(CurfewEventType.Day);
        var chats = await this.alarmBotContext.Chats.ToListAsync();

        var chatsToBlockDuringCurfew = chats.Where(currentChat => currentChat.Settings.BlockChatDuringCurfew);
        foreach (var chatToBlockDuringCurfew in chatsToBlockDuringCurfew)
        {
            if (!chatToBlockDuringCurfew.Status.BlockedDuringAlarm)
            {
                await this.safeTelegramClient.UnblockChatAsync(chatToBlockDuringCurfew.TelegramId);

                ////var message = await this.safeTelegramClient.SendTextMessageAsync(chatToBlockDuringCurfew.TelegramId, AppSettings.CurfewUnblockText);
                ////curfewLog.AddMessage(message, chatToBlockDuringCurfew);
            }

            chatToBlockDuringCurfew.Status.BlockedDuringCurfew = false;
        }

        await this.alarmBotContext.AddAsync(curfewLog);
        await this.alarmBotContext.SaveChangesAsync();

        await this.safeTelegramClient.SendTextMessageAsync(AppSettings.AdminChatId, AppSettings.CurfewMessageSentText(curfewLog.Id));
    }

    public async Task RemoveCurfewLogAsync(int curfewLogId)
    {
        var curfewLog = await this.alarmBotContext.CurfewLogs
            .Include(curfewLog => curfewLog.CurfewLogMessages)
            .ThenInclude(dbCurfewLogMessage => dbCurfewLogMessage.Chat)
            .FirstAsync(curfewLog => curfewLog.Id == curfewLogId);

        if (!curfewLog.IsDeleted)
        {
            foreach (var curfewLogMessage in curfewLog.CurfewLogMessages)
            {
                await this.safeTelegramClient.DeleteTelegramMessage(curfewLogMessage.Chat!.TelegramId, curfewLogMessage.MessageId);
            }
        }

        curfewLog.MarkAsDeleted();
        await this.alarmBotContext.SaveChangesAsync();
    }
}