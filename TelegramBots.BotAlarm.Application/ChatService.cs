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
        var chat = await this.telegramBotClient.GetChatAsync(telegramId);

        var existingDbChat = await this.alarmBotContext.Chats.WithTelegramId(telegramId);
        if (existingDbChat != null)
        {
            return Result<DbChat>.Fail();
        }

        var dbChat = new DbChat(chat);
        await this.alarmBotContext.Chats.AddAsync(dbChat);
        await this.alarmBotContext.SaveChangesAsync();

        return Result<DbChat>.Ok(dbChat);
    }

    public async Task<Result<DbChat>> RemoveChatAsync(long telegramId)
    {
        var existingDbChat = await this.alarmBotContext.Chats.WithTelegramId(telegramId);
        if (existingDbChat == null)
        {
            return Result<DbChat>.Fail();
        }

        this.alarmBotContext.Remove(existingDbChat);
        await this.alarmBotContext.SaveChangesAsync();

        return Result<DbChat>.Ok(existingDbChat);
    }

    public async Task<Result<DbChat>> UpdateChatSettings(long chatTelegramId, Action<ChatSettings> action)
    {
        var existingDbChat = await this.alarmBotContext.Chats.WithTelegramId(chatTelegramId);
        if (existingDbChat == null)
        {
            return Result<DbChat>.Fail();
        }

        action(existingDbChat.Settings);

        await this.alarmBotContext.SaveChangesAsync();

        return Result<DbChat>.Ok(existingDbChat);
    }
}