using Microsoft.EntityFrameworkCore;

using TelegramBots.BotAlarm.Domain;

namespace TelegramBots.BotAlarm.Application;

public class AlarmNotificationService : IAlarmNotificationService
{
    private readonly ISafeTelegramClient safeTelegramClient;
    private readonly AlarmBotContext alarmBotContext;

    public AlarmNotificationService(ISafeTelegramClient safeTelegramClient, AlarmBotContext alarmBotContext)
    {
        this.safeTelegramClient = safeTelegramClient;
        this.alarmBotContext = alarmBotContext;
    }

    public async Task NotifyAlarmAsync()
    {
        var alarmLog = new AlarmLog(AlarmEventType.Alarm);
        var chats = await alarmBotContext.Chats.ToListAsync().ConfigureAwait(false);

        var chatsToBroadcast = chats.Where(currentChat => currentChat.Settings.BroadcastMessageDuringAlarm);
        foreach (var chatToBroadcast in chatsToBroadcast)
        {
            var message = await safeTelegramClient.SendTextMessageAsync(chatToBroadcast.TelegramId, AppSettings.AlarmText).ConfigureAwait(false);
            alarmLog.AddMessage(message, chatToBroadcast);
        }

        var chatsToBlockDuringAlarm = chats.Where(currentChat => currentChat.Settings.BlockChatDuringAlarm);
        foreach (var chatToBlockDuringAlarm in chatsToBlockDuringAlarm)
        {
            if (!chatToBlockDuringAlarm.Status.BlockedDuringCurfew)
            {
                await safeTelegramClient.BlockChatAsync(chatToBlockDuringAlarm.TelegramId).ConfigureAwait(false);

                ////var message = await this.safeTelegramClient.SendTextMessageAsync(chatToBlockDuringAlarm.TelegramId, AppSettings.BlockText).ConfigureAwait(false);
                ////alarmLog.AddMessage(message, chatToBlockDuringAlarm);
            }

            chatToBlockDuringAlarm.Status.BlockedDuringAlarm = true;
        }

        await alarmBotContext.AddAsync(alarmLog).ConfigureAwait(false);
        await alarmBotContext.SaveChangesAsync().ConfigureAwait(false);

        await safeTelegramClient.SendTextMessageAsync(AppSettings.AdminChatId, AppSettings.AlarmMessageSentText(alarmLog.Id)).ConfigureAwait(false);
    }

    public async Task NotifyContinuationAsync()
    {
        var alarmLog = new AlarmLog(AlarmEventType.Continue);
        var chats = await alarmBotContext.Chats.ToListAsync().ConfigureAwait(false);

        var chatsToBroadcast = chats.Where(currentChat => currentChat.Settings.BroadcastMessageDuringAlarm);
        foreach (var chatToBroadcast in chatsToBroadcast)
        {
            var message = await safeTelegramClient.SendTextMessageAsync(chatToBroadcast.TelegramId, AppSettings.ContinueText).ConfigureAwait(false);
            alarmLog.AddMessage(message, chatToBroadcast);
        }

        await alarmBotContext.AddAsync(alarmLog).ConfigureAwait(false);
        await alarmBotContext.SaveChangesAsync().ConfigureAwait(false);

        await safeTelegramClient.SendTextMessageAsync(AppSettings.AdminChatId, AppSettings.AlarmMessageSentText(alarmLog.Id)).ConfigureAwait(false);
    }

    public async Task NotifyRejectAsync()
    {
        var alarmLog = new AlarmLog(AlarmEventType.Reject);
        var chats = await alarmBotContext.Chats.ToListAsync().ConfigureAwait(false);

        var chatsToBroadcast = chats.Where(currentChat => currentChat.Settings.BroadcastMessageDuringAlarm);
        foreach (var chatToBroadcast in chatsToBroadcast)
        {
            var message = await safeTelegramClient.SendTextMessageAsync(chatToBroadcast.TelegramId, AppSettings.RejectText).ConfigureAwait(false);
            alarmLog.AddMessage(message, chatToBroadcast);
        }

        var chatsToBlockDuringAlarm = chats.Where(currentChat => currentChat.Settings.BlockChatDuringAlarm);
        foreach (var chatToBlockDuringAlarm in chatsToBlockDuringAlarm)
        {
            if (!chatToBlockDuringAlarm.Status.BlockedDuringCurfew)
            {
                await safeTelegramClient.UnblockChatAsync(chatToBlockDuringAlarm.TelegramId).ConfigureAwait(false);

                ////var message = await this.safeTelegramClient.SendTextMessageAsync(chatToBlockDuringAlarm.TelegramId, AppSettings.UnblockText).ConfigureAwait(false);
                ////alarmLog.AddMessage(message, chatToBlockDuringAlarm);
            }

            chatToBlockDuringAlarm.Status.BlockedDuringAlarm = false;
        }

        await alarmBotContext.AddAsync(alarmLog).ConfigureAwait(false);
        await alarmBotContext.SaveChangesAsync().ConfigureAwait(false);

        await safeTelegramClient.SendTextMessageAsync(AppSettings.AdminChatId, AppSettings.AlarmMessageSentText(alarmLog.Id)).ConfigureAwait(false);
    }

    ////public async Task NotifyQuiteTimeAsync(int hoursWithoutAlarm)
    ////{
    ////    var alarmLog = new AlarmLog(AlarmEventType.QuiteTime);

    ////    await this.safeTelegramClient.SendTextMessageAsync(AppSettings.BoId, AppSettings.HoursWithoutAlarmText(hoursWithoutAlarm)).ConfigureAwait(false);

    ////    await this.alarmBotContext.AddAsync(alarmLog).ConfigureAwait(false);
    ////    await this.alarmBotContext.SaveChangesAsync().ConfigureAwait(false);
    ////}

    public Task NotifyQuiteTimeAsync(int hoursWithoutAlarm)
    {
        return Task.FromResult(0);
    }

    public async Task NotifyTestAsync()
    {
        var alarmLog = new AlarmLog(AlarmEventType.QuiteTime);
        var chats = await alarmBotContext.Chats.ToListAsync().ConfigureAwait(false);

        var chatsToBroadcast = chats.Where(currentChat => currentChat.Settings.BroadcastMessageDuringAlarm);
        foreach (var chatToBroadcast in chatsToBroadcast.Where(chat => chat.TelegramId == AppSettings.AdminChatId))
        {
            var message = await safeTelegramClient.SendTextMessageAsync(chatToBroadcast.TelegramId, AppSettings.AlarmText).ConfigureAwait(false);
            alarmLog.AddMessage(message, chatToBroadcast);
        }

        await alarmBotContext.AddAsync(alarmLog).ConfigureAwait(false);
        await alarmBotContext.SaveChangesAsync().ConfigureAwait(false);

        await safeTelegramClient.SendTextMessageAsync(AppSettings.AdminChatId, AppSettings.AlarmMessageSentText(alarmLog.Id)).ConfigureAwait(false);
    }
}