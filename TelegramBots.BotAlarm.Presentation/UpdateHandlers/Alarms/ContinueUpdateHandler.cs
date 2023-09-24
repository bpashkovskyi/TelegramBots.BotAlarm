namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Alarms;

[MessageShouldBeCommand("continue")]
[AllowedMessageTypes(MessageType.Text)]
[AllowedChats(AppSettings.AdminChatId)]
public class ContinueUpdateHandler : UpdateHandler
{
    private readonly IAlarmService _alarmService;
    private readonly IBotService botService;

    public ContinueUpdateHandler(
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
        await this._alarmService.NotifyContinuationAsync();
        await this.botService.StopAutomaticChecking();
    }
}