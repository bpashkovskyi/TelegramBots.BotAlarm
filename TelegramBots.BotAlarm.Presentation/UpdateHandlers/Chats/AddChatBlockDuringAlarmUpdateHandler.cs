namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Chats;

[MessageShouldBeCommand("blockon")]
public class AddChatBlockDuringAlarmUpdateHandler : UpdateChatSettingUpdateHandler
{
    public AddChatBlockDuringAlarmUpdateHandler(IRollbar rollbar, ITelegramBotClient telegramBotClient, IChatService chatService)
       : base(rollbar, telegramBotClient, chatService) { }

    public override async Task HandleAsync(Update update)
    {
        await this.UpdateChatSettingsAsync(update, chatSettings => chatSettings.BlockChatDuringAlarm = true);
    }
}