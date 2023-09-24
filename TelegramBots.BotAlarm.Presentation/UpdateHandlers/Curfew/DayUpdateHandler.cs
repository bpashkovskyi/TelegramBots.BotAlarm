namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Curfew;

[MessageShouldBeCommand("day")]
[AllowedMessageTypes(MessageType.Text)]
[AllowedChats(AppSettings.AdminChatId)]
public class DayUpdateHandler : UpdateHandler
{
    private readonly ICurfewNotificationService curfewNotificationService;
    private readonly IBotService botService;

    public DayUpdateHandler(
        IRollbar rollbar,
        ITelegramBotClient telegramBotClient,
        ICurfewNotificationService curfewNotificationService,
        IBotService botService)
        : base(rollbar, telegramBotClient)
    {
        this.curfewNotificationService = curfewNotificationService;
        this.botService = botService;
    }

    public override async Task HandleAsync(Update update)
    {
        await this.curfewNotificationService.NotifyDayAsync().ConfigureAwait(false);
        await this.botService.StopAutomaticChecking().ConfigureAwait(false);
    }
}