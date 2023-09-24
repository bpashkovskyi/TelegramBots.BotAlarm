namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Chats;

[MessageShouldBeCommand("curfewoff")]
public class RemoveChatBlockDuringCurfewUpdateHandler : UpdateChatSettingUpdateHandler
{
    public RemoveChatBlockDuringCurfewUpdateHandler(IRollbar rollbar, ITelegramBotClient telegramBotClient, IChatService chatService)
        : base(rollbar, telegramBotClient, chatService) { }

    public override async Task HandleAsync(Update update)
    {
        await this.UpdateChatSettingsAsync(update, chatSettings => chatSettings.BlockChatDuringCurfew = false);
    }
}