namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Alarms;

[MessageShouldBeCommand("test")]
[AllowedMessageTypes(MessageType.Text)]
[AllowedChats(AppSettings.AdminChatId)]
public class TestUpdateHandler : UpdateHandler
{
    private readonly IAlarmService _alarmService;
    private readonly IBotService botService;

    public TestUpdateHandler(
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
        await this._alarmService.NotifyTestAsync();
        await this.botService.StopAutomaticChecking();
    }
}