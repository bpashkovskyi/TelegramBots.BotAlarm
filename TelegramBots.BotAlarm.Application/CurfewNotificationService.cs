using Microsoft.EntityFrameworkCore;

using TelegramBots.BotAlarm.Domain;

namespace TelegramBots.BotAlarm.Application;

public class CurfewNotificationService : ICurfewNotificationService
{
    private readonly ISafeTelegramClient safeTelegramClient;
    private readonly AlarmBotContext alarmBotContext;

    public CurfewNotificationService(ISafeTelegramClient safeTelegramClient, AlarmBotContext alarmBotContext)
    {
        this.safeTelegramClient = safeTelegramClient;
        this.alarmBotContext = alarmBotContext;
    }

    public async Task NotifyNightAsync()
    {
        var curfewLog = new CurfewLog(CurfewEventType.Night);
        var chats = await alarmBotContext.Chats.ToListAsync().ConfigureAwait(false);

        var chatsToBlockDuringCurfew = chats.Where(currentChat => currentChat.Settings.BlockChatDuringCurfew);
        foreach (var chatToBlockDuringCurfew in chatsToBlockDuringCurfew)
        {
            if (!chatToBlockDuringCurfew.Status.BlockedDuringAlarm)
            {
                await safeTelegramClient.BlockChatAsync(chatToBlockDuringCurfew.TelegramId).ConfigureAwait(false);

                ////var message = await this.safeTelegramClient.SendTextMessageAsync(chatToBlockDuringCurfew.TelegramId, AppSettings.CurfewBlockText).ConfigureAwait(false);
                ////curfewLog.AddMessage(message, chatToBlockDuringCurfew);
            }

            chatToBlockDuringCurfew.Status.BlockedDuringCurfew = true;
        }

        await alarmBotContext.AddAsync(curfewLog).ConfigureAwait(false);
        await alarmBotContext.SaveChangesAsync().ConfigureAwait(false);

        await safeTelegramClient.SendTextMessageAsync(AppSettings.AdminChatId, AppSettings.CurfewMessageSentText(curfewLog.Id)).ConfigureAwait(false);
    }

    public async Task NotifyDayAsync()
    {
        var curfewLog = new CurfewLog(CurfewEventType.Day);
        var chats = await alarmBotContext.Chats.ToListAsync().ConfigureAwait(false);

        var chatsToBlockDuringCurfew = chats.Where(currentChat => currentChat.Settings.BlockChatDuringCurfew);
        foreach (var chatToBlockDuringCurfew in chatsToBlockDuringCurfew)
        {
            if (!chatToBlockDuringCurfew.Status.BlockedDuringAlarm)
            {
                await safeTelegramClient.UnblockChatAsync(chatToBlockDuringCurfew.TelegramId).ConfigureAwait(false);

                ////var message = await this.safeTelegramClient.SendTextMessageAsync(chatToBlockDuringCurfew.TelegramId, AppSettings.CurfewUnblockText).ConfigureAwait(false);
                ////curfewLog.AddMessage(message, chatToBlockDuringCurfew);
            }

            chatToBlockDuringCurfew.Status.BlockedDuringCurfew = false;
        }

        await alarmBotContext.AddAsync(curfewLog).ConfigureAwait(false);
        await alarmBotContext.SaveChangesAsync().ConfigureAwait(false);

        await safeTelegramClient.SendTextMessageAsync(AppSettings.AdminChatId, AppSettings.CurfewMessageSentText(curfewLog.Id)).ConfigureAwait(false);
    }
}