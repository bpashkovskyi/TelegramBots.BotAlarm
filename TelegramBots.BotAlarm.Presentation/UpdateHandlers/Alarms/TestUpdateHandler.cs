namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Alarms;

[MessageShouldBeCommand("test")]
[AllowedMessageTypes(MessageType.Text)]
[AllowedChats(AppSettings.AdminChatId)]
public class TestUpdateHandler : UpdateHandler
{
    private readonly IAlarmNotificationService alarmNotificationService;
    private readonly IBotService botService;

    public TestUpdateHandler(
        IRollbar rollbar,
        ITelegramBotClient telegramBotClient,
        IAlarmNotificationService alarmNotificationService,
        IBotService botService)
        : base(rollbar, telegramBotClient)
    {
        this.alarmNotificationService = alarmNotificationService;
        this.botService = botService;
    }

    public override async Task HandleAsync(Update update)
    {
        await this.alarmNotificationService.NotifyTestAsync().ConfigureAwait(false);
        await this.botService.StopAutomaticChecking().ConfigureAwait(false);
    }
}