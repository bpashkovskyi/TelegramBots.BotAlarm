namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Curfew;

[MessageShouldBeCommand("day")]
[AllowedMessageTypes(MessageType.Text)]
[AllowedChats(AppSettings.AdminChatId)]
public class DayUpdateHandler : UpdateHandler
{
    private readonly ICurfewService curfewService;
    private readonly IBotService botService;

    public DayUpdateHandler(
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
        await this.curfewService.NotifyDayAsync();
        await this.botService.StopAutomaticChecking();
    }
}