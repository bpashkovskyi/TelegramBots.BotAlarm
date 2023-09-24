using TelegramBots.BotAlarm.Persistence;

namespace TelegramBots.BotAlarm.UnitTests;

using Microsoft.EntityFrameworkCore;

public static class DbContextFactory
{
    public static AlarmBotContext Create()
    {
        var options = new DbContextOptionsBuilder<AlarmBotContext>()
            .UseInMemoryDatabase(databaseName: "AlarmBotConnection")
            .Options;

        var alarmBotContext = new AlarmBotContext(options);

        return alarmBotContext;
    }
}