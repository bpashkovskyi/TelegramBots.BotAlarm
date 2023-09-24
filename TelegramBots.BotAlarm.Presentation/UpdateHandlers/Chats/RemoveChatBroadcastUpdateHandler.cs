namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Chats;

[MessageShouldBeCommand("alarmoff")]
public class RemoveChatBroadcastUpdateHandler : UpdateChatSettingUpdateHandler
{
    public RemoveChatBroadcastUpdateHandler(IRollbar rollbar, ITelegramBotClient telegramBotClient, IChatService chatService)
        : base(rollbar, telegramBotClient, chatService) { }

    public override async Task HandleAsync(Update update)
    {
        await this.UpdateChatSettingsAsync(update, chatSettings => chatSettings.BroadcastMessageDuringAlarm = false).ConfigureAwait(false);
    }
}