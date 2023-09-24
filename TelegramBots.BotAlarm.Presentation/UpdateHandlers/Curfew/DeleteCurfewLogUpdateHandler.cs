using TelegramBots.Shared.Extensions;

namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Curfew;

[MessageShouldBeCommand("deletecurfew")]
[AllowedMessageTypes(MessageType.Text)]
[AllowedChats(AppSettings.AdminChatId)]
public class DeleteCurfewLogUpdateHandler : UpdateHandler
{
    private readonly ICurfewService curfewService;

    public DeleteCurfewLogUpdateHandler(IRollbar rollbar, ITelegramBotClient telegramBotClient, ICurfewService curfewService)
        : base(rollbar, telegramBotClient)
    {
        this.curfewService = curfewService;
    }

    public override async Task HandleAsync(Update update)
    {
        var message = update.Message!;

        var curfewLogId = message.TextAsInt();
        if (curfewLogId != null)
        {
            await this.curfewService.RemoveCurfewLogAsync(curfewLogId.Value).ConfigureAwait(false);
        }
    }
}