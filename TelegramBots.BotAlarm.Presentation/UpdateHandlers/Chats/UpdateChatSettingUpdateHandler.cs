﻿using Newtonsoft.Json;

using TelegramBots.BotAlarm.Domain.Model.ValueObjects;
using TelegramBots.Shared.Extensions;

namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Chats;

public class UpdateChatSettingUpdateHandler : UpdateHandler
{
    private readonly IChatService chatService;

    public UpdateChatSettingUpdateHandler(IRollbar rollbar, ITelegramBotClient telegramBotClient, IChatService chatService)
        : base(rollbar, telegramBotClient)
    {
        this.chatService = chatService;
    }

    public async Task UpdateChatSettingsAsync(Update update, Action<ChatSettings> action)
    {
        var message = update.Message!;

        this.Rollbar.Info(message.GetSenderInfo());

        // God mode
        if (message.Chat.Id == AppSettings.BoId)
        {
            var chatId = message.TextAsLong();
            if (chatId != null)
            {
                await this.UpdateChatSettingsAsync(message.Chat.Id, chatId.Value, action);
            }

            return;
        }

        var memberInfo = await this.TelegramBotClient.GetChatMemberAsync(message.Chat.Id, message.From!.Id);
        if (memberInfo.Status is not (ChatMemberStatus.Administrator or ChatMemberStatus.Creator))
        {
            await this.TelegramBotClient.SendTextMessageAsync(message.Chat.Id, "Вам потрібні права адміністратора");
            return;
        }

        await this.UpdateChatSettingsAsync(message.Chat.Id, message.Chat.Id, action);
    }

    private async Task UpdateChatSettingsAsync(long messageChatId, long targetChatId, Action<ChatSettings> action)
    {
        var updateSettingsResult = await this.chatService.UpdateChatSettings(targetChatId, action);
        if (updateSettingsResult.Success)
        {
            await this.TelegramBotClient.SendTextMessageAsync(messageChatId, $"Налаштування чату оновлено: {JsonConvert.SerializeObject(updateSettingsResult.Value!.Settings, Formatting.Indented)}");
        }
        else
        {
            await this.TelegramBotClient.SendTextMessageAsync(messageChatId, "Чат не знайдено");
        }
    }

    public override Task HandleAsync(Update update)
    {
        return Task.FromResult(0);
    }
}