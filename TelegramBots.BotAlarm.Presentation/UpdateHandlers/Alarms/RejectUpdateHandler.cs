namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Alarms;

[MessageShouldBeCommand("reject")]
[AllowedMessageTypes(MessageType.Text)]
[AllowedChats(AppSettings.AdminChatId)]
public class RejectUpdateHandler : UpdateHandler
{
    private readonly IAlarmNotificationService alarmNotificationService;
    private readonly IBotService botService;

    public RejectUpdateHandler(
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
        await this.alarmNotificationService.NotifyRejectAsync().ConfigureAwait(false);
        await this.botService.StopAutomaticChecking().ConfigureAwait(false);
    }
}