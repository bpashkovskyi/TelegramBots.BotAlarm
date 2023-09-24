namespace TelegramBots.BotAlarm.Application.Base;

public interface IAlarmService
{
    Task RemoveAlarmLogAsync(int alarmLogId);
}