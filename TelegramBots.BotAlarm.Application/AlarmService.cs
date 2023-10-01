using Microsoft.EntityFrameworkCore;

using TelegramBots.BotAlarm.Persistence;

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

    public async Task NotifyAlarmAsync()
    {
        var alarmLog = new AlarmLog(AlarmEventType.Alarm);
        var chats = await this.alarmBotContext.Chats.ToListAsync().ConfigureAwait(false);

        var chatsToBroadcast = chats.Where(currentChat => currentChat.Settings.BroadcastMessageDuringAlarm);
        foreach (var chatToBroadcast in chatsToBroadcast)
        {
            var message = await this.safeTelegramClient.SendTextMessageAsync(chatToBroadcast.TelegramId, AppSettings.AlarmText).ConfigureAwait(false);
            alarmLog.AddMessage(message, chatToBroadcast);
        }

        var chatsToBlockDuringAlarm = chats.Where(currentChat => currentChat.Settings.BlockChatDuringAlarm);
        foreach (var chatToBlockDuringAlarm in chatsToBlockDuringAlarm)
        {
            if (!chatToBlockDuringAlarm.Status.BlockedDuringCurfew)
            {
                await this.safeTelegramClient.BlockChatAsync(chatToBlockDuringAlarm.TelegramId).ConfigureAwait(false);

                ////var message = await this.safeTelegramClient.SendTextMessageAsync(chatToBlockDuringAlarm.TelegramId, AppSettings.BlockText).ConfigureAwait(false);
                ////alarmLog.AddMessage(message, chatToBlockDuringAlarm);
            }

            chatToBlockDuringAlarm.Status.BlockedDuringAlarm = true;
        }

        await this.alarmBotContext.AddAsync(alarmLog).ConfigureAwait(false);
        await this.alarmBotContext.SaveChangesAsync().ConfigureAwait(false);

        await this.safeTelegramClient.SendTextMessageAsync(AppSettings.AdminChatId, AppSettings.AlarmMessageSentText(alarmLog.Id)).ConfigureAwait(false);
    }

    public async Task NotifyContinuationAsync()
    {
        var alarmLog = new AlarmLog(AlarmEventType.Continue);
        var chats = await this.alarmBotContext.Chats.ToListAsync().ConfigureAwait(false);

        var chatsToBroadcast = chats.Where(currentChat => currentChat.Settings.BroadcastMessageDuringAlarm);
        foreach (var chatToBroadcast in chatsToBroadcast)
        {
            var message = await this.safeTelegramClient.SendTextMessageAsync(chatToBroadcast.TelegramId, AppSettings.ContinueText).ConfigureAwait(false);
            alarmLog.AddMessage(message, chatToBroadcast);
        }

        await this.alarmBotContext.AddAsync(alarmLog).ConfigureAwait(false);
        await this.alarmBotContext.SaveChangesAsync().ConfigureAwait(false);

        await this.safeTelegramClient.SendTextMessageAsync(AppSettings.AdminChatId, AppSettings.AlarmMessageSentText(alarmLog.Id)).ConfigureAwait(false);
    }

    public async Task NotifyRejectAsync()
    {
        var alarmLog = new AlarmLog(AlarmEventType.Reject);
        var chats = await this.alarmBotContext.Chats.ToListAsync().ConfigureAwait(false);

        var chatsToBroadcast = chats.Where(currentChat => currentChat.Settings.BroadcastMessageDuringAlarm);
        foreach (var chatToBroadcast in chatsToBroadcast)
        {
            var message = await this.safeTelegramClient.SendTextMessageAsync(chatToBroadcast.TelegramId, AppSettings.RejectText).ConfigureAwait(false);
            alarmLog.AddMessage(message, chatToBroadcast);
        }

        var chatsToBlockDuringAlarm = chats.Where(currentChat => currentChat.Settings.BlockChatDuringAlarm);
        foreach (var chatToBlockDuringAlarm in chatsToBlockDuringAlarm)
        {
            if (!chatToBlockDuringAlarm.Status.BlockedDuringCurfew)
            {
                await this.safeTelegramClient.UnblockChatAsync(chatToBlockDuringAlarm.TelegramId).ConfigureAwait(false);

                ////var message = await this.safeTelegramClient.SendTextMessageAsync(chatToBlockDuringAlarm.TelegramId, AppSettings.UnblockText).ConfigureAwait(false);
                ////alarmLog.AddMessage(message, chatToBlockDuringAlarm);
            }

            chatToBlockDuringAlarm.Status.BlockedDuringAlarm = false;
        }

        await this.alarmBotContext.AddAsync(alarmLog).ConfigureAwait(false);
        await this.alarmBotContext.SaveChangesAsync().ConfigureAwait(false);

        await this.safeTelegramClient.SendTextMessageAsync(AppSettings.AdminChatId, AppSettings.AlarmMessageSentText(alarmLog.Id)).ConfigureAwait(false);
    }

    public async Task NotifyTestAsync()
    {
        var alarmLog = new AlarmLog(AlarmEventType.QuiteTime);
        var chats = await this.alarmBotContext.Chats.ToListAsync().ConfigureAwait(false);

        var chatsToBroadcast = chats.Where(currentChat => currentChat.Settings.BroadcastMessageDuringAlarm);
        foreach (var chatToBroadcast in chatsToBroadcast.Where(chat => chat.TelegramId == AppSettings.AdminChatId))
        {
            var message = await this.safeTelegramClient.SendTextMessageAsync(chatToBroadcast.TelegramId, AppSettings.AlarmText).ConfigureAwait(false);
            alarmLog.AddMessage(message, chatToBroadcast);
        }

        await this.alarmBotContext.AddAsync(alarmLog).ConfigureAwait(false);
        await this.alarmBotContext.SaveChangesAsync().ConfigureAwait(false);

        await this.safeTelegramClient.SendTextMessageAsync(AppSettings.AdminChatId, AppSettings.AlarmMessageSentText(alarmLog.Id)).ConfigureAwait(false);
    }

    public async Task RemoveAlarmLogAsync(int alarmLogId)
    {
        var alarmLog = await this.alarmBotContext.AlarmLogs
            .Include(alarmLog => alarmLog.AlarmLogMessages)
            .ThenInclude(dbAlarmLogMessage => dbAlarmLogMessage.Chat)
            .FirstAsync(alarmLog => alarmLog.Id == alarmLogId).ConfigureAwait(false);

        if (!alarmLog.IsDeleted)
        {
            foreach (var alarmLogMessage in alarmLog.AlarmLogMessages)
            {
                await this.safeTelegramClient.DeleteTelegramMessage(alarmLogMessage.Chat!.TelegramId, alarmLogMessage.MessageId).ConfigureAwait(false);
            }

            alarmLog.MarkAsDeleted();
            await this.alarmBotContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}