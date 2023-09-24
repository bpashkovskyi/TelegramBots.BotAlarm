using TelegramBots.BotAlarm.Domain.Model.Entities;

namespace TelegramBots.BotAlarm.Persistence.Base;

public interface IChatRepository
{
    Task<List<DbChat>> GetChatsToBroadcastMessageDuringAlarmAsync();
    Task<List<DbChat>> GetChatsToBlockDuringCurfewAsync();
    Task<List<DbChat>> GetChatsToBlockDuringAlarmAsync();
}