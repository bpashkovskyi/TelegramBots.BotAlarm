namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Chats;

[MessageShouldBeCommand("alarmon")]
public class AddChatBroadcastUpdateHandler : UpdateChatSettingUpdateHandler
{
    public AddChatBroadcastUpdateHandler(IRollbar rollbar, ITelegramBotClient telegramBotClient, IChatService chatService)
        : base(rollbar, telegramBotClient, chatService) { }

    public override async Task HandleAsync(Update update)
    {
        await this.UpdateChatSettingsAsync(update, chatSettings => chatSettings.BroadcastMessageDuringAlarm = true).ConfigureAwait(false);
    }
}