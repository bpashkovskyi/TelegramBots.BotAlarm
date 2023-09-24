namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Alarms;

[MessageShouldBeCommand("reject")]
[AllowedMessageTypes(MessageType.Text)]
[AllowedChats(AppSettings.AdminChatId)]
public class RejectUpdateHandler : UpdateHandler
{
    private readonly IAlarmService _alarmService;
    private readonly IBotService botService;

    public RejectUpdateHandler(
        IRollbar rollbar,
        ITelegramBotClient telegramBotClient,
        IAlarmService alarmService,
        IBotService botService)
        : base(rollbar, telegramBotClient)
    {
        this._alarmService = alarmService;
        this.botService = botService;
    }

    public override async Task HandleAsync(Update update)
    {
        await this._alarmService.NotifyRejectAsync();
        await this.botService.StopAutomaticChecking();
    }
}