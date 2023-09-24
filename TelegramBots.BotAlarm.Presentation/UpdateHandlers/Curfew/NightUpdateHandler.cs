namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Curfew;

[MessageShouldBeCommand("night")]
[AllowedMessageTypes(MessageType.Text)]
[AllowedChats(AppSettings.AdminChatId)]
public class NightUpdateHandler : UpdateHandler
{
    private readonly ICurfewService curfewService;
    private readonly IBotService botService;

    public NightUpdateHandler(
        IRollbar rollbar,
        ITelegramBotClient telegramBotClient,
        ICurfewService curfewService,
        IBotService botService)
        : base(rollbar, telegramBotClient)
    {
        this.curfewService = curfewService;
        this.botService = botService;
    }

    public override async Task HandleAsync(Update update)
    {
        await this.curfewService.NotifyNightAsync();
        await this.botService.StopAutomaticChecking();
    }
}