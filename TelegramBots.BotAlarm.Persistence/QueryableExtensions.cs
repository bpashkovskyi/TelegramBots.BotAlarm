using Microsoft.EntityFrameworkCore;

using TelegramBots.BotAlarm.Domain.Model.Entities;
using TelegramBots.BotAlarm.Domain.Model.Enums;

namespace TelegramBots.BotAlarm.Persistence;

public static class QueryableExtensions
{
    public static async Task<DbChat?> WithTelegramId(this IQueryable<DbChat> chats, long telegramId)
    {
        return await chats.FirstOrDefaultAsync(chat => chat.TelegramId == telegramId);
    }

    public static async Task<AlarmLog?> Last(this IQueryable<AlarmLog> alarmLogs)
    {
        return await alarmLogs.OrderByDescending(alarmLog => alarmLog.DateTime).FirstOrDefaultAsync();
    }

    public static async Task<CurfewLog?> Last(this IQueryable<CurfewLog> curfewLogs)
    {
        return await curfewLogs.OrderByDescending(curfewLog => curfewLog.DateTime).FirstOrDefaultAsync();
    }

    public static async Task<ServiceLog?> Last(this IQueryable<ServiceLog> serviceLogs)
    {
        return await serviceLogs.OrderByDescending(serviceLog => serviceLog.DateTime).FirstOrDefaultAsync();
    }

    public static IQueryable<AlarmLog> NotDeleted(this IQueryable<AlarmLog> alarmLogs)
    {
        return alarmLogs.Where(alarmLog => !alarmLog.IsDeleted);
    }

    public static IQueryable<CurfewLog> NotDeleted(this IQueryable<CurfewLog> curfewLogs)
    {
        return curfewLogs.Where(curfewLog => !curfewLog.IsDeleted);
    }

    public static IQueryable<AlarmLog> WithEventType(this IQueryable<AlarmLog> alarmLogs, AlarmEventType eventType)
    {
        return alarmLogs.Where(alarmLog => alarmLog.EventType == eventType);
    }
}