namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Bot;

[MessageShouldBeCommand("stop")]
[AllowedMessageTypes(MessageType.Text)]
[AllowedChats(AppSettings.AdminChatId)]
public class StopAutomaticCheckingUpdateHandler : UpdateHandler
{
    private readonly IBotService botService;

    public StopAutomaticCheckingUpdateHandler(
        IRollbar rollbar,
        ITelegramBotClient telegramBotClient,
        IBotService botService)
        : base(rollbar, telegramBotClient)
    {
        this.botService = botService;
    }

    public override async Task HandleAsync(Update update)
    {
        await this.botService.StopAutomaticChecking();
    }
}