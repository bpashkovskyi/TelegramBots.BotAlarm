namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Curfew;

[MessageShouldBeCommand("night")]
[AllowedMessageTypes(MessageType.Text)]
[AllowedChats(AppSettings.AdminChatId)]
public class NightUpdateHandler : UpdateHandler
{
    private readonly ICurfewService _curfewService;
    private readonly IBotService botService;

    public NightUpdateHandler(
        IRollbar rollbar,
        ITelegramBotClient telegramBotClient,
        ICurfewService curfewService,
        IBotService botService)
        : base(rollbar, telegramBotClient)
    {
        this._curfewService = curfewService;
        this.botService = botService;
    }

    public override async Task HandleAsync(Update update)
    {
        await this._curfewService.NotifyNightAsync();
        await this.botService.StopAutomaticChecking();
    }
}