﻿namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Alarms;

[MessageShouldBeCommand("alarm")]
[AllowedMessageTypes(MessageType.Text)]
[AllowedChats(AppSettings.AdminChatId)]
public class AlarmUpdateHandler : UpdateHandler
{
    private readonly IAlarmService _alarmService;
    private readonly IBotService botService;

    public AlarmUpdateHandler(
        IRollbar rollbar,
        ITelegramBotClient telegramBotClient,
        IAlarmService alarmService,
        IBotService botService)
        : base(rollbar, telegramBotClient)
    {
        this._alarmService = alarmService;
        this.botService = botService;
    }

    public override async Task HandleAsync(Update update)
    {
        await this._alarmService.NotifyAlarmAsync();
        await this.botService.StopAutomaticChecking();
    }
}