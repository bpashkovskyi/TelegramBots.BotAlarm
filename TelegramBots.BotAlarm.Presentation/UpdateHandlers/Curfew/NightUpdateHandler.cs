namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Curfew;

[MessageShouldBeCommand("night")]
[AllowedMessageTypes(MessageType.Text)]
[AllowedChats(AppSettings.AdminChatId)]
public class NightUpdateHandler : UpdateHandler
{
    private readonly ICurfewNotificationService curfewNotificationService;
    private readonly IBotService botService;

    public NightUpdateHandler(
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
        await this.curfewNotificationService.NotifyNightAsync().ConfigureAwait(false);
        await this.botService.StopAutomaticChecking().ConfigureAwait(false);
    }
}