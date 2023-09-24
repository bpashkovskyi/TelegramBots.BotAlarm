namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Alarms;

[MessageShouldBeCommand("alarm")]
[AllowedMessageTypes(MessageType.Text)]
[AllowedChats(AppSettings.AdminChatId)]
public class AlarmUpdateHandler : UpdateHandler
{
    private readonly IAlarmService alarmService;
    private readonly IBotService botService;

    public AlarmUpdateHandler(
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
        await this.alarmService.NotifyAlarmAsync();
        await this.botService.StopAutomaticChecking();
    }
}