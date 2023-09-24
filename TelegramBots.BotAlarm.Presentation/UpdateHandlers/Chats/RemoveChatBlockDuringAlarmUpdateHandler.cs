namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Chats;

[MessageShouldBeCommand("blockoff")]
public class RemoveChatBlockDuringAlarmUpdateHandler : UpdateChatSettingUpdateHandler
{
    public RemoveChatBlockDuringAlarmUpdateHandler(IRollbar rollbar, ITelegramBotClient telegramBotClient, IChatService chatService)
        : base(rollbar, telegramBotClient, chatService) { }

    public override async Task HandleAsync(Update update)
    {
        await this.UpdateChatSettingsAsync(update, chatSettings => chatSettings.BlockChatDuringAlarm = false);
    }
}