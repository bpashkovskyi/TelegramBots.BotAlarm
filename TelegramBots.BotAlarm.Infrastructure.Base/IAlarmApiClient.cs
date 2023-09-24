namespace TelegramBots.BotAlarm.Infrastructure.Base;

using System.Collections.Generic;
using System.Threading.Tasks;

public interface IAlarmApiClient
{
    Task<Dictionary<string, bool>?> GetRegionAlarmsAsync();
}