namespace TelegramBots.BotAlarm.Infrastructure;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Rollbar;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

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
            return await this.telegramBotClient.SendTextMessageAsync(chatId, messageText, ParseMode.Html);
        }
        catch (Exception exception)
        {
            this.rollbar.Critical(
                exception,
                new Dictionary<string, object?>
                {
                    { "chat", chatId },
                });

            return null;
        }
    }

    public async Task<Message?> SendStickerAsync(long chatId, string fileId)
    {
        try
        {
            return await this.telegramBotClient.SendStickerAsync(chatId, new InputOnlineFile(fileId));
        }
        catch (Exception exception)
        {
            this.rollbar.Critical(
                exception,
                new Dictionary<string, object?>
                {
                    { "chat", chatId },
                });

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
                    CanSendMediaMessages = false,
                });
        }
        catch (Exception exception)
        {
            this.rollbar.Critical(
                exception,
                new Dictionary<string, object?>
                {
                    { "chat", chatId },
                });
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
                    CanSendMessages = true,
                    CanSendMediaMessages = true,
                });
        }
        catch (Exception exception)
        {
            this.rollbar.Critical(
                exception,
                new Dictionary<string, object?>
                {
                    { "chat", chatId },
                });
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
            this.rollbar.Critical(
                exception,
                new Dictionary<string, object?>
                {
                    { "chat", chatId },
                });
        }
    }
}