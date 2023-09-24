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

        await this.alarmBotContext.ServiceLogs.AddAsync(serviceLog);
        await this.alarmBotContext.SaveChangesAsync();

        await this.safeTelegramClient.SendTextMessageAsync(AppSettings.AdminChatId, AppSettings.BotStoppedMessage);
    }

    public async Task StartAutomaticChecking()
    {
        var serviceLog = new ServiceLog(ServiceType.Start);

        await this.alarmBotContext.ServiceLogs.AddAsync(serviceLog);
        await this.alarmBotContext.SaveChangesAsync();

        await this.safeTelegramClient.SendTextMessageAsync(AppSettings.AdminChatId, AppSettings.BotStartedMessage);
    }
}