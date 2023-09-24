namespace TelegramBots.BotAlarm.Application;

public class AlarmService : IAlarmService
{
    private readonly ISafeTelegramClient safeTelegramClient;
    private readonly IAlarmLogRepository alarmLogRepository;
    private readonly IChatRepository chatRepository;

    public AlarmService(
        ISafeTelegramClient safeTelegramClient,
        IAlarmLogRepository alarmLogRepository,
        IChatRepository chatRepository)
    {
        this.safeTelegramClient = safeTelegramClient;
        this.alarmLogRepository = alarmLogRepository;
        this.chatRepository = chatRepository;
    }

    public async Task NotifyAlarmAsync()
    {
        var alarmLog = new AlarmLog(AlarmEventType.Alarm);
        
        var chatsToBroadcast = await this.chatRepository.GetChatsToBroadcastMessageDuringAlarmAsync();
        foreach (var chatToBroadcast in chatsToBroadcast)
        {
            var message = await this.safeTelegramClient.SendTextMessageAsync(chatToBroadcast.TelegramId, AppSettings.AlarmText);
            alarmLog.AddMessage(message, chatToBroadcast);
        }

        var chatsToBlockDuringAlarm = await this.chatRepository.GetChatsToBlockDuringAlarmAsync();
        foreach (var chatToBlockDuringAlarm in chatsToBlockDuringAlarm)
        {
            if (!chatToBlockDuringAlarm.Status.BlockedDuringCurfew)
            {
                await this.safeTelegramClient.BlockChatAsync(chatToBlockDuringAlarm.TelegramId);

                ////var message = await this.safeTelegramClient.SendTextMessageAsync(chatToBlockDuringAlarm.TelegramId, AppSettings.BlockText);
                ////alarmLog.AddMessage(message, chatToBlockDuringAlarm);
            }

            chatToBlockDuringAlarm.Status.BlockedDuringAlarm = true;
        }

        await this.alarmLogRepository.AddLogAsync(alarmLog);
        await this.alarmLogRepository.SaveChangesAsync();

        await this.safeTelegramClient.SendTextMessageAsync(AppSettings.AdminChatId, AppSettings.AlarmMessageSentText(alarmLog.Id));
    }

    public async Task NotifyContinuationAsync()
    {
        var alarmLog = new AlarmLog(AlarmEventType.Continue);

        var chatsToBroadcast = await this.chatRepository.GetChatsToBroadcastMessageDuringAlarmAsync();
        foreach (var chatToBroadcast in chatsToBroadcast)
        {
            var message = await this.safeTelegramClient.SendTextMessageAsync(chatToBroadcast.TelegramId, AppSettings.ContinueText);
            alarmLog.AddMessage(message, chatToBroadcast);
        }

        await this.alarmLogRepository.AddLogAsync(alarmLog);
        await this.alarmLogRepository.SaveChangesAsync();

        await this.safeTelegramClient.SendTextMessageAsync(AppSettings.AdminChatId, AppSettings.AlarmMessageSentText(alarmLog.Id));
    }

    public async Task NotifyRejectAsync()
    {
        var alarmLog = new AlarmLog(AlarmEventType.Reject);

        var chatsToBroadcast = await this.chatRepository.GetChatsToBroadcastMessageDuringAlarmAsync();
        foreach (var chatToBroadcast in chatsToBroadcast)
        {
            var message = await this.safeTelegramClient.SendTextMessageAsync(chatToBroadcast.TelegramId, AppSettings.RejectText);
            alarmLog.AddMessage(message, chatToBroadcast);
        }

        var chatsToBlockDuringAlarm = await this.chatRepository.GetChatsToBlockDuringAlarmAsync();
        foreach (var chatToBlockDuringAlarm in chatsToBlockDuringAlarm)
        {
            if (!chatToBlockDuringAlarm.Status.BlockedDuringCurfew)
            {
                await this.safeTelegramClient.UnblockChatAsync(chatToBlockDuringAlarm.TelegramId);

                ////var message = await this.safeTelegramClient.SendTextMessageAsync(chatToBlockDuringAlarm.TelegramId, AppSettings.UnblockText);
                ////alarmLog.AddMessage(message, chatToBlockDuringAlarm);
            }

            chatToBlockDuringAlarm.Status.BlockedDuringAlarm = false;
        }

        await this.alarmLogRepository.AddLogAsync(alarmLog);
        await this.alarmLogRepository.SaveChangesAsync();

        await this.safeTelegramClient.SendTextMessageAsync(AppSettings.AdminChatId, AppSettings.AlarmMessageSentText(alarmLog.Id));
    }

    ////public async Task NotifyQuiteTimeAsync(int hoursWithoutAlarm)
    ////{
    ////    var alarmLog = new AlarmLog(AlarmEventType.QuiteTime);

    ////    await this.safeTelegramClient.SendTextMessageAsync(AppSettings.BoId, AppSettings.HoursWithoutAlarmText(hoursWithoutAlarm));

    ////    await this.alarmBotContext.AddAsync(alarmLog);
    ////    await this.alarmBotContext.SaveChangesAsync();
    ////}

    public Task NotifyQuiteTimeAsync(int hoursWithoutAlarm)
    {
        return Task.FromResult(0);
    }

    public async Task NotifyTestAsync()
    {
        var alarmLog = new AlarmLog(AlarmEventType.QuiteTime);

        var chatsToBroadcast = await this.chatRepository.GetChatsToBroadcastMessageDuringAlarmAsync();
        foreach (var chatToBroadcast in chatsToBroadcast.Where(chat => chat.TelegramId == AppSettings.AdminChatId))
        {
            var message = await this.safeTelegramClient.SendTextMessageAsync(chatToBroadcast.TelegramId, AppSettings.AlarmText);
            alarmLog.AddMessage(message, chatToBroadcast);
        }

        await this.alarmLogRepository.AddLogAsync(alarmLog);
        await this.alarmLogRepository.SaveChangesAsync();

        await this.safeTelegramClient.SendTextMessageAsync(AppSettings.AdminChatId, AppSettings.AlarmMessageSentText(alarmLog.Id));
    }

    public async Task RemoveAlarmLogAsync(int alarmLogId)
    {
        var alarmLog = await this.alarmLogRepository.GetLogAsync(alarmLogId);

        if (!alarmLog.IsDeleted)
        {
            foreach (var alarmLogMessage in alarmLog.AlarmLogMessages)
            {
                await this.safeTelegramClient.DeleteTelegramMessage(alarmLogMessage.Chat!.TelegramId, alarmLogMessage.MessageId);
            }

            alarmLog.MarkAsDeleted();
            await this.alarmLogRepository.SaveChangesAsync();
        }
    }
}