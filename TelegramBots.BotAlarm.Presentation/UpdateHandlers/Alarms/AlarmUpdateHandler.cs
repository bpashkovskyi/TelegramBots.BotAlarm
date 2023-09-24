namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Alarms;

[MessageShouldBeCommand("alarm")]
[AllowedMessageTypes(MessageType.Text)]
[AllowedChats(AppSettings.AdminChatId)]
public class AlarmUpdateHandler : UpdateHandler
{
    private readonly IAlarmNotificationService alarmNotificationService;
    private readonly IBotService botService;

    public AlarmUpdateHandler(
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
        await this.alarmNotificationService.NotifyAlarmAsync().ConfigureAwait(false);
        await this.botService.StopAutomaticChecking().ConfigureAwait(false);
    }
}