namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers;

[AllowedUpdateType(UpdateType.Message, "^/broadcast (?<message>.+)$")]
[AllowedChats(AppSettings.BoId)]
public class BroadcastMessageUpdateHandler : UpdateHandler
{
    private readonly IAlarmService alarmService;

    public BroadcastMessageUpdateHandler(
        IRollbar rollbar,
        ITelegramBotClient telegramBotClient,
        IAlarmService alarmService)
        : base(rollbar, telegramBotClient)
    {
        this.alarmService = alarmService;
    }

    public override async Task HandleAsync(Update update)
    {
        var messageText = this.Arguments["message"];

        await this.alarmService.BroadcastMessageAsync(messageText);
    }
}