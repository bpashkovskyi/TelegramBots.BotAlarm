using TelegramBots.Shared.Extensions;

namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Chats;

[MessageShouldBeCommand("removechat")]
public class RemoveChatUpdateHandler : UpdateHandler
{
    private readonly IChatService chatService;

    public RemoveChatUpdateHandler(IRollbar rollbar, ITelegramBotClient telegramBotClient, IChatService chatService)
        : base(rollbar, telegramBotClient)
    {
        this.chatService = chatService;
    }

    public override async Task HandleAsync(Update update)
    {
        var message = update.Message!;

        this.Rollbar.Info(message.GetSenderInfo());

        // God mode
        if (message.Chat.Id == AppSettings.BoId)
        {
            var chatId = message.TextAsLong();
            if (chatId != null)
            {
                await this.RemoveChatAsync(message.Chat.Id, chatId.Value);
            }

            return;
        }

        var memberInfo = await this.TelegramBotClient.GetChatMemberAsync(message.Chat.Id, message.From!.Id);
        if (memberInfo.Status is not (ChatMemberStatus.Administrator or ChatMemberStatus.Creator))
        {
            await this.TelegramBotClient.SendTextMessageAsync(message.Chat.Id, "Вам потрібні права адміністратора");
            return;
        }

        await this.RemoveChatAsync(message.Chat.Id, message.Chat.Id);
    }

    private async Task RemoveChatAsync(long messageChatId, long targetChatId)
    {
        var removeChatResult = await this.chatService.RemoveChatAsync(targetChatId);
        if (removeChatResult.Success)
        {
            await this.TelegramBotClient.SendTextMessageAsync(messageChatId, "Чат видалено");
        }
        else
        {
            await this.TelegramBotClient.SendTextMessageAsync(messageChatId, "Чату не існує");
        }
    }
}