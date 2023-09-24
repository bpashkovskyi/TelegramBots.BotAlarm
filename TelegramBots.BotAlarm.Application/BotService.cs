using TelegramBots.BotAlarm.Domain;

namespace TelegramBots.BotAlarm.Application;

public class BotService : IBotService
{
    private readonly ISafeTelegramClient safeTelegramClient;
    private readonly AlarmBotContext alarmBotContext;

    public BotService(ISafeTelegramClient safeTelegramClient, AlarmBotContext alarmBotContext)
    {
        this.safeTelegramClient = safeTelegramClient;
        this.alarmBotContext = alarmBotContext;
    }

    public async Task StopAutomaticChecking()
    {
        var serviceLog = new ServiceLog(ServiceType.Stop);

        await alarmBotContext.ServiceLogs.AddAsync(serviceLog).ConfigureAwait(false);
        await alarmBotContext.SaveChangesAsync().ConfigureAwait(false);

        await safeTelegramClient.SendTextMessageAsync(AppSettings.AdminChatId, AppSettings.BotStoppedMessage).ConfigureAwait(false);
    }

    public async Task StartAutomaticChecking()
    {
        var serviceLog = new ServiceLog(ServiceType.Start);

        await alarmBotContext.ServiceLogs.AddAsync(serviceLog).ConfigureAwait(false);
        await alarmBotContext.SaveChangesAsync().ConfigureAwait(false);

        await safeTelegramClient.SendTextMessageAsync(AppSettings.AdminChatId, AppSettings.BotStartedMessage).ConfigureAwait(false);
    }
}