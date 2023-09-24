namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Chats;

[MessageShouldBeCommand("curfewon")]
public class AddChatBlockDuringCurfewUpdateHandler : UpdateChatSettingUpdateHandler
{
    public AddChatBlockDuringCurfewUpdateHandler(IRollbar rollbar, ITelegramBotClient telegramBotClient, IChatService chatService)
        : base(rollbar, telegramBotClient, chatService)
    {
    }

    public override async Task HandleAsync(Update update)
    {
        await this.UpdateChatSettingsAsync(update, chatSettings => chatSettings.BlockChatDuringCurfew = true).ConfigureAwait(false);
    }
}