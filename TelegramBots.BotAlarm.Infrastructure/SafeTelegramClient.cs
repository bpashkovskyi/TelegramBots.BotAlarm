namespace TelegramBots.BotAlarm.Infrastructure;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Rollbar;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using TelegramBots.BotAlarm.Infrastructure.Base;

public class SafeTelegramClient : ISafeTelegramClient
{
    private readonly IRollbar rollbar;
    private readonly ITelegramBotClient telegramBotClient;

    public SafeTelegramClient(IRollbar rollbar, ITelegramBotClient telegramBotClient)
    {
        this.rollbar = rollbar;
        this.telegramBotClient = telegramBotClient;
    }

    public async Task<Message?> SendTextMessageAsync(long chatId, string messageText)
    {
        try
        {
            return await this.telegramBotClient.SendTextMessageAsync(chatId, messageText, parseMode: ParseMode.Html);
        }
        catch (Exception exception)
        {
            this.LogCriticalError(chatId, exception);

            return null;
        }
    }

    public async Task BlockChatAsync(long chatId)
    {
        try
        {
            await this.telegramBotClient.SetChatPermissionsAsync(
                chatId,
                new ChatPermissions
                {
                    CanSendMessages = false,
                    CanSendPhotos = false,
                    CanSendVideos = false
                });
        }
        catch (Exception exception)
        {
            this.LogCriticalError(chatId, exception);
        }
    }

    public async Task UnblockChatAsync(long chatId)
    {
        try
        {
            await this.telegramBotClient.SetChatPermissionsAsync(
                chatId,
                new ChatPermissions
                {
                    CanSendMessages = false,
                    CanSendPhotos = false,
                    CanSendVideos = false
                });
        }
        catch (Exception exception)
        {
            this.LogCriticalError(chatId, exception);
        }
    }

    public async Task DeleteTelegramMessage(long chatId, int messageId)
    {
        try
        {
            await this.telegramBotClient.DeleteMessageAsync(chatId, messageId);
        }
        catch (Exception exception)
        {
            this.LogCriticalError(chatId, exception);
        }
    }

    private void LogCriticalError(long chatId, Exception exception)
    {
        this.rollbar.Critical(exception, new Dictionary<string, object?>
        {
            { "chat", chatId },
        });
    }
}