using TelegramBots.Shared.Extensions;

namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Chats;

[MessageShouldBeCommand("addchat")]
public class AddChatUpdateHandler : UpdateHandler
{
    private readonly IChatService chatService;

    public AddChatUpdateHandler(IRollbar rollbar, ITelegramBotClient telegramBotClient, IChatService chatService)
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
                await this.AddChatAsync(message.Chat.Id, chatId.Value).ConfigureAwait(false);
            }

            return;
        }

        var memberInfo = await this.TelegramBotClient.GetChatMemberAsync(message.Chat.Id, message.From!.Id).ConfigureAwait(false);
        if (memberInfo.Status is not (ChatMemberStatus.Administrator or ChatMemberStatus.Creator))
        {
            await this.TelegramBotClient.SendTextMessageAsync(message.Chat.Id, "Вам потрібні права адміністратора").ConfigureAwait(false);
            return;
        }

        var chatMembersCount = await this.TelegramBotClient.GetChatMemberCountAsync(message.Chat.Id).ConfigureAwait(false);
        if (chatMembersCount < 500)
        {
            await this.TelegramBotClient.SendTextMessageAsync(message.Chat.Id, "Бот може бути доданий тільки у чат з кількістю підписників не менше 500").ConfigureAwait(false);
            return;
        }

        await this.AddChatAsync(message.Chat.Id, message.Chat.Id).ConfigureAwait(false);
    }

    private async Task AddChatAsync(long messageChatId, long targetChatId)
    {
        var addChatResult = await this.chatService.AddChatAsync(targetChatId).ConfigureAwait(false);
        if (addChatResult.Success)
        {
            await this.TelegramBotClient.SendTextMessageAsync(messageChatId, "Чат додано").ConfigureAwait(false);
        }
        else
        {
            await this.TelegramBotClient.SendTextMessageAsync(messageChatId, "Чат вже існує").ConfigureAwait(false);
        }
    }
}