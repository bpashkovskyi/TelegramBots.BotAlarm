﻿namespace TelegramBots.BotAlarm.Infrastructure.Base;

using System.Threading.Tasks;

using Telegram.Bot.Types;

public interface ISafeTelegramClient
{
    Task<Message?> SendTextMessageAsync(long chatId, string messageText);

    Task<Message?> SendStickerAsync(long chatId, string fileId);

    Task BlockChatAsync(long chatId);

    Task UnblockChatAsync(long chatId);

    Task DeleteTelegramMessage(long chatId, int messageId);
}