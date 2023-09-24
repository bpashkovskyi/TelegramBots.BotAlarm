using TelegramBots.Shared.Extensions;

namespace TelegramBots.BotAlarm.Presentation.UpdateHandlers.Alarms;

[MessageShouldBeCommand("deletealarm")]
[AllowedMessageTypes(MessageType.Text)]
[AllowedChats(AppSettings.AdminChatId)]
public class DeleteAlarmLogUpdateHandler : UpdateHandler
{
    private readonly IAlarmService alarmService;

    public DeleteAlarmLogUpdateHandler(IRollbar rollbar, ITelegramBotClient telegramBotClient, IAlarmService alarmService)
        : base(rollbar, telegramBotClient)
    {
        this.alarmService = alarmService;
    }

    public override async Task HandleAsync(Update update)
    {
        var message = update.Message!;

        var alarmLogId = message.TextAsInt();
        if (alarmLogId != null)
        {
            await this.alarmService.RemoveAlarmLogAsync(alarmLogId.Value);
        }
    }
}