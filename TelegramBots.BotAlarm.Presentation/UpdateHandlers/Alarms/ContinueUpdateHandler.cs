namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Alarms;

[MessageShouldBeCommand("continue")]
[AllowedMessageTypes(MessageType.Text)]
[AllowedChats(AppSettings.AdminChatId)]
public class ContinueUpdateHandler : UpdateHandler
{
    private readonly IAlarmNotificationService alarmNotificationService;
    private readonly IBotService botService;

    public ContinueUpdateHandler(
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
        await this.alarmNotificationService.NotifyContinuationAsync().ConfigureAwait(false);
        await this.botService.StopAutomaticChecking().ConfigureAwait(false);
    }
}