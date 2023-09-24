namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Bot;

[MessageShouldBeCommand("start")]
[AllowedMessageTypes(MessageType.Text)]
[AllowedChats(AppSettings.AdminChatId)]
public class StartAutomaticCheckingUpdateHandler : UpdateHandler
{
    private readonly IBotService botService;

    public StartAutomaticCheckingUpdateHandler(
        IRollbar rollbar,
        ITelegramBotClient telegramBotClient,
        IBotService botService)
        : base(rollbar, telegramBotClient)
    {
        this.botService = botService;
    }

    public override async Task HandleAsync(Update update)
    {
        await this.botService.StartAutomaticChecking().ConfigureAwait(false);
    }
}