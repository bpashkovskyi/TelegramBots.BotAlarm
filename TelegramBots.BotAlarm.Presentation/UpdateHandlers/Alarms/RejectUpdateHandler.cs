namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Alarms;

[MessageShouldBeCommand("reject")]
[AllowedMessageTypes(MessageType.Text)]
[AllowedChats(AppSettings.AdminChatId)]
public class RejectUpdateHandler : UpdateHandler
{
    private readonly IAlarmService alarmService;
    private readonly IBotService botService;

    public RejectUpdateHandler(
        IRollbar rollbar,
        ITelegramBotClient telegramBotClient,
        IAlarmService alarmService,
        IBotService botService)
        : base(rollbar, telegramBotClient)
    {
        this.alarmService = alarmService;
        this.botService = botService;
    }

    public override async Task HandleAsync(Update update)
    {
        await this.alarmService.NotifyRejectAsync();
        await this.botService.StopAutomaticChecking();
    }
}