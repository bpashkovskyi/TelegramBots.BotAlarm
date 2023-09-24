using Telegram.Bot;

using TelegramBots.BotAlarm.Domain.Model.ValueObjects;
using TelegramBots.Shared;

namespace TelegramBots.BotAlarm.Application;

public class ChatService : IChatService
{
    private readonly ITelegramBotClient telegramBotClient;
    private readonly AlarmBotContext alarmBotContext;

    public ChatService(ITelegramBotClient telegramBotClient, AlarmBotContext alarmBotContext)
    {
        this.telegramBotClient = telegramBotClient;
        this.alarmBotContext = alarmBotContext;
    }

    public async Task<Result<DbChat>> AddChatAsync(long telegramId)
    {
        var chat = await telegramBotClient.GetChatAsync(telegramId).ConfigureAwait(false);

        var existingDbChat = await alarmBotContext.Chats.WithTelegramId(telegramId).ConfigureAwait(false);
        if (existingDbChat != null)
        {
            return Result<DbChat>.Fail();
        }

        var dbChat = new DbChat(chat);
        await alarmBotContext.Chats.AddAsync(dbChat).ConfigureAwait(false);
        await alarmBotContext.SaveChangesAsync().ConfigureAwait(false);

        return Result<DbChat>.Ok(dbChat);
    }

    public async Task<Result<DbChat>> RemoveChatAsync(long telegramId)
    {
        var existingDbChat = await alarmBotContext.Chats.WithTelegramId(telegramId).ConfigureAwait(false);
        if (existingDbChat == null)
        {
            return Result<DbChat>.Fail();
        }

        alarmBotContext.Remove(existingDbChat);
        await alarmBotContext.SaveChangesAsync().ConfigureAwait(false);

        return Result<DbChat>.Ok(existingDbChat);
    }

    public async Task<Result<DbChat>> UpdateChatSettings(long chatTelegramId, Action<ChatSettings> action)
    {
        var existingDbChat = await alarmBotContext.Chats.WithTelegramId(chatTelegramId).ConfigureAwait(false);
        if (existingDbChat == null)
        {
            return Result<DbChat>.Fail();
        }

        action(existingDbChat.Settings);

        await alarmBotContext.SaveChangesAsync().ConfigureAwait(false);

        return Result<DbChat>.Ok(existingDbChat);
    }
}