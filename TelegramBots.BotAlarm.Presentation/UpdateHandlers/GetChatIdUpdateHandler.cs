using System.Globalization;

namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers;

[MessageShouldBeCommand("getchatid")]
public class GetChatIdUpdateHandler : UpdateHandler
{
    public GetChatIdUpdateHandler(IRollbar rollbar, ITelegramBotClient telegramBotClient)
        : base(rollbar, telegramBotClient)
    {
    }

    public override async Task HandleAsync(Update update)
    {
        var message = update.Message!;

        var chatIdAsString = message.Chat.Id.ToString(CultureInfo.InvariantCulture);

        await this.TelegramBotClient.SendTextMessageAsync(message.Chat.Id, chatIdAsString).ConfigureAwait(false);
    }
}