using Microsoft.EntityFrameworkCore;

using TelegramBots.BotAlarm.Domain.Model.Entities;
using TelegramBots.BotAlarm.Persistence.Base;

namespace TelegramBots.BotAlarm.Persistence.Repositories;

class ChatRepository : IChatRepository
{
    private readonly AlarmBotContext alarmBotContext;

    public ChatRepository(AlarmBotContext alarmBotContext)
    {
        this.alarmBotContext = alarmBotContext;
    }

    public async Task<List<DbChat>> GetChatsToBroadcastMessageDuringAlarmAsync()
    {
        return await this.alarmBotContext.Chats.Where(currentChat => currentChat.Settings.BroadcastMessageDuringAlarm).ToListAsync();
    }

    public async Task<List<DbChat>> GetChatsToBlockDuringCurfewAsync()
    {
        return await this.alarmBotContext.Chats.Where(currentChat => currentChat.Settings.BlockChatDuringCurfew).ToListAsync();
    }

    public async Task<List<DbChat>> GetChatsToBlockDuringAlarmAsync()
    {
        return await this.alarmBotContext.Chats.Where(currentChat => currentChat.Settings.BlockChatDuringAlarm).ToListAsync();
    }
}