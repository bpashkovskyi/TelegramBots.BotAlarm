using TelegramBots.BotAlarm.Domain.Model.Entities;
using TelegramBots.BotAlarm.Domain.Model.ValueObjects;
using TelegramBots.Shared;

namespace TelegramBots.BotAlarm.Application.Base;

public interface IChatService
{
    Task<Result<DbChat>> AddChatAsync(long telegramId);

    Task<Result<DbChat>> RemoveChatAsync(long telegramId);

    Task<Result<DbChat>> UpdateChatSettings(long chatTelegramId, Action<ChatSettings> action);
}