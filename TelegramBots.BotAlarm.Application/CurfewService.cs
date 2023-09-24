namespace TelegramBots.BotAlarm.Application;

public class CurfewService : ICurfewService
{
    private readonly ISafeTelegramClient safeTelegramClient;
    private readonly ICurfewLogRepository curfewLogRepository;
    private readonly IChatRepository chatRepository;

    public CurfewService(
        ISafeTelegramClient safeTelegramClient,
        ICurfewLogRepository curfewLogRepository,
        IChatRepository chatRepository)
    {
        this.safeTelegramClient = safeTelegramClient;
        this.curfewLogRepository = curfewLogRepository;
        this.chatRepository = chatRepository;
    }

    public async Task NotifyNightAsync()
    {
        var curfewLog = new CurfewLog(CurfewEventType.Night);

        var chatsToBlockDuringCurfew = await this.chatRepository.GetChatsToBlockDuringCurfewAsync();
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

        await this.curfewLogRepository.AddLogAsync(curfewLog);
        await this.curfewLogRepository.SaveChangesAsync();
        
        await this.safeTelegramClient.SendTextMessageAsync(AppSettings.AdminChatId, AppSettings.CurfewMessageSentText(curfewLog.Id));
    }

    public async Task NotifyDayAsync()
    {
        var curfewLog = new CurfewLog(CurfewEventType.Day);

        var chatsToBlockDuringCurfew = await this.chatRepository.GetChatsToBlockDuringCurfewAsync();
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

        await this.curfewLogRepository.AddLogAsync(curfewLog);
        await this.curfewLogRepository.SaveChangesAsync();

        await this.safeTelegramClient.SendTextMessageAsync(AppSettings.AdminChatId, AppSettings.CurfewMessageSentText(curfewLog.Id));
    }

    public async Task RemoveCurfewLogAsync(int curfewLogId)
    {
        var curfewLog = await this.curfewLogRepository.GetLogAsync(curfewLogId);

        if (!curfewLog.IsDeleted)
        {
            foreach (var curfewLogMessage in curfewLog.CurfewLogMessages)
            {
                await this.safeTelegramClient.DeleteTelegramMessage(curfewLogMessage.Chat!.TelegramId, curfewLogMessage.MessageId);
            }
        }

        curfewLog.MarkAsDeleted();
        await this.curfewLogRepository.SaveChangesAsync();
    }
}