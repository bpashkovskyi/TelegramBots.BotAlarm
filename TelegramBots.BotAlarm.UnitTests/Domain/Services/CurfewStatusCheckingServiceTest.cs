using TelegramBots.BotAlarm.Application.Base;
using TelegramBots.BotAlarm.Domain;
using TelegramBots.BotAlarm.Domain.Model.Entities;
using TelegramBots.BotAlarm.Domain.Model.Enums;

namespace TelegramBots.BotAlarm.UnitTests.Domain.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using Rollbar;

[TestClass]
public class CurfewStatusCheckingServiceTest
{
    private readonly DateTime Day = new DateTime(2022, 1, 1, 15, 0, 0);
    private readonly DateTime Night = new DateTime(2022, 1, 1, 3, 0, 0);

    [TestInitialize]
    public async Task TestInitialize()
    {
        var alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext.ConfigureAwait(false))
        {
            await alarmBotContext.Database.EnsureDeletedAsync().ConfigureAwait(false);
            await alarmBotContext.Database.EnsureCreatedAsync().ConfigureAwait(false);
        }
    }

    [TestMethod]
    public async Task CheckForCurfewAndNotifyShouldNotDoAnythingIfLastServiceLogIsNull()
    {
        var alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext.ConfigureAwait(false))
        {
            alarmBotContext.ServiceLogs.Add(new ServiceLog(ServiceType.Stop));
            await alarmBotContext.SaveChangesAsync().ConfigureAwait(false);
        }

        var rollbarMock = new Mock<IRollbar>(MockBehavior.Strict);
        var curfewNotificationServiceMock = new Mock<ICurfewNotificationService>(MockBehavior.Strict);

        alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext.ConfigureAwait(false))
        {
            var curfewStatusCheckingService = new CurfewStatusCheckingService(rollbarMock.Object, curfewNotificationServiceMock.Object, alarmBotContext);

            await curfewStatusCheckingService.CheckForCurfewAndNotifyAsync(DateTime.UtcNow).ConfigureAwait(false);
        }
    }

    [TestMethod]
    public async Task CheckForCurfewAndNotifyShouldNotifyNightIfIsNightAndLastCurfewLogIsNull()
    {
        var rollbarMock = new Mock<IRollbar>(MockBehavior.Strict);
        var curfewNotificationServiceMock = new Mock<ICurfewNotificationService>(MockBehavior.Strict);

        curfewNotificationServiceMock.Setup(curfewNotificationService => curfewNotificationService.NotifyNightAsync())
            .Returns(Task.FromResult(0));

        var alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext.ConfigureAwait(false))
        {
            var curfewStatusCheckingService = new CurfewStatusCheckingService(rollbarMock.Object, curfewNotificationServiceMock.Object, alarmBotContext);

            await curfewStatusCheckingService.CheckForCurfewAndNotifyAsync(Night).ConfigureAwait(false);

            curfewNotificationServiceMock.Verify(curfewNotificationService => curfewNotificationService.NotifyNightAsync());
        }
    }

    [TestMethod]
    public async Task CheckForCurfewAndNotifyShouldNotifyNightIfIsNightAndLastCurfewLogIsDay()
    {
        var alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext.ConfigureAwait(false))
        {
            alarmBotContext.CurfewLogs.Add(new CurfewLog(CurfewEventType.Day));
            await alarmBotContext.SaveChangesAsync().ConfigureAwait(false);
        }

        var rollbarMock = new Mock<IRollbar>(MockBehavior.Strict);
        var curfewNotificationServiceMock = new Mock<ICurfewNotificationService>(MockBehavior.Strict);

        curfewNotificationServiceMock.Setup(curfewNotificationService => curfewNotificationService.NotifyNightAsync())
            .Returns(Task.FromResult(0));

        alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext.ConfigureAwait(false))
        {
            var curfewStatusCheckingService = new CurfewStatusCheckingService(rollbarMock.Object, curfewNotificationServiceMock.Object, alarmBotContext);

            await curfewStatusCheckingService.CheckForCurfewAndNotifyAsync(Night).ConfigureAwait(false);

            curfewNotificationServiceMock.Verify(curfewNotificationService => curfewNotificationService.NotifyNightAsync());
        }
    }

    [TestMethod]
    public async Task CheckForCurfewAndNotifyShouldDoAnythingIfIsNightAndLastCurfewLogIsNight()
    {
        var alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext.ConfigureAwait(false))
        {
            alarmBotContext.CurfewLogs.Add(new CurfewLog(CurfewEventType.Night));
            await alarmBotContext.SaveChangesAsync().ConfigureAwait(false);
        }

        var rollbarMock = new Mock<IRollbar>(MockBehavior.Strict);
        var curfewNotificationServiceMock = new Mock<ICurfewNotificationService>(MockBehavior.Strict);

        alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext.ConfigureAwait(false))
        {
            var curfewStatusCheckingService = new CurfewStatusCheckingService(rollbarMock.Object, curfewNotificationServiceMock.Object, alarmBotContext);

            await curfewStatusCheckingService.CheckForCurfewAndNotifyAsync(Night).ConfigureAwait(false);
        }
    }

    [TestMethod]
    public async Task CheckForCurfewAndNotifyShouldShouldLogExceptionIfIsNotifyNightFailed()
    {
        var alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext.ConfigureAwait(false))
        {
            alarmBotContext.CurfewLogs.Add(new CurfewLog(CurfewEventType.Day));
            await alarmBotContext.SaveChangesAsync().ConfigureAwait(false);
        }

        var rollbarMock = new Mock<IRollbar>(MockBehavior.Strict);
        var curfewNotificationServiceMock = new Mock<ICurfewNotificationService>(MockBehavior.Strict);

        rollbarMock.Setup(rollbar => rollbar.Critical(It.IsAny<InvalidOperationException>(), It.IsAny<Dictionary<string, object?>>()))
            .Returns(rollbarMock.Object);

        curfewNotificationServiceMock.Setup(curfewNotificationService => curfewNotificationService.NotifyNightAsync())
            .ThrowsAsync(new InvalidOperationException());

        alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext.ConfigureAwait(false))
        {
            var curfewStatusCheckingService = new CurfewStatusCheckingService(rollbarMock.Object, curfewNotificationServiceMock.Object, alarmBotContext);

            await curfewStatusCheckingService.CheckForCurfewAndNotifyAsync(Night).ConfigureAwait(false);

            rollbarMock.Verify(rollbar => rollbar.Critical(It.IsAny<InvalidOperationException>(), It.IsAny<Dictionary<string, object?>>()));
        }
    }

    [TestMethod]
    public async Task CheckForCurfewAndNotifyShouldNotifyDayIfIsDayAndLastCurfewLogIsNull()
    {
        var rollbarMock = new Mock<IRollbar>(MockBehavior.Strict);
        var curfewNotificationServiceMock = new Mock<ICurfewNotificationService>(MockBehavior.Strict);

        curfewNotificationServiceMock.Setup(curfewNotificationService => curfewNotificationService.NotifyDayAsync())
            .Returns(Task.FromResult(0));

        var alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext.ConfigureAwait(false))
        {
            var curfewStatusCheckingService = new CurfewStatusCheckingService(rollbarMock.Object, curfewNotificationServiceMock.Object, alarmBotContext);

            await curfewStatusCheckingService.CheckForCurfewAndNotifyAsync(Day).ConfigureAwait(false);

            curfewNotificationServiceMock.Verify(curfewNotificationService => curfewNotificationService.NotifyDayAsync());
        }
    }

    [TestMethod]
    public async Task CheckForCurfewAndNotifyShouldNotifyDayIfIsDayAndLastCurfewLogIsNight()
    {
        var alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext.ConfigureAwait(false))
        {
            alarmBotContext.CurfewLogs.Add(new CurfewLog(CurfewEventType.Night));
            await alarmBotContext.SaveChangesAsync().ConfigureAwait(false);
        }

        var rollbarMock = new Mock<IRollbar>(MockBehavior.Strict);
        var curfewNotificationServiceMock = new Mock<ICurfewNotificationService>(MockBehavior.Strict);

        curfewNotificationServiceMock.Setup(curfewNotificationService => curfewNotificationService.NotifyDayAsync())
            .Returns(Task.FromResult(0));

        alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext.ConfigureAwait(false))
        {
            var curfewStatusCheckingService = new CurfewStatusCheckingService(rollbarMock.Object, curfewNotificationServiceMock.Object, alarmBotContext);

            await curfewStatusCheckingService.CheckForCurfewAndNotifyAsync(Day).ConfigureAwait(false);

            curfewNotificationServiceMock.Verify(curfewNotificationService => curfewNotificationService.NotifyDayAsync());
        }
    }

    [TestMethod]
    public async Task CheckForCurfewAndNotifyShouldDoAnythingIfIsDayAndLastCurfewLogIsDay()
    {
        var alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext.ConfigureAwait(false))
        {
            alarmBotContext.CurfewLogs.Add(new CurfewLog(CurfewEventType.Day));
            await alarmBotContext.SaveChangesAsync().ConfigureAwait(false);
        }

        var rollbarMock = new Mock<IRollbar>(MockBehavior.Strict);
        var curfewNotificationServiceMock = new Mock<ICurfewNotificationService>(MockBehavior.Strict);

        alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext.ConfigureAwait(false))
        {
            var curfewStatusCheckingService = new CurfewStatusCheckingService(rollbarMock.Object, curfewNotificationServiceMock.Object, alarmBotContext);

            await curfewStatusCheckingService.CheckForCurfewAndNotifyAsync(Day).ConfigureAwait(false);
        }
    }

    [TestMethod]
    public async Task CheckForCurfewAndNotifyShouldShouldLogExceptionIfIsNotifyDayFailed()
    {
        var alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext.ConfigureAwait(false))
        {
            alarmBotContext.CurfewLogs.Add(new CurfewLog(CurfewEventType.Night));
            await alarmBotContext.SaveChangesAsync().ConfigureAwait(false);
        }

        var rollbarMock = new Mock<IRollbar>(MockBehavior.Strict);
        var curfewNotificationServiceMock = new Mock<ICurfewNotificationService>(MockBehavior.Strict);

        rollbarMock.Setup(rollbar => rollbar.Critical(It.IsAny<InvalidOperationException>(), It.IsAny<Dictionary<string, object?>>()))
            .Returns(rollbarMock.Object);

        curfewNotificationServiceMock.Setup(curfewNotificationService => curfewNotificationService.NotifyDayAsync())
            .ThrowsAsync(new InvalidOperationException());

        alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext.ConfigureAwait(false))
        {
            var curfewStatusCheckingService = new CurfewStatusCheckingService(rollbarMock.Object, curfewNotificationServiceMock.Object, alarmBotContext);

            await curfewStatusCheckingService.CheckForCurfewAndNotifyAsync(Day).ConfigureAwait(false);

            rollbarMock.Verify(rollbar => rollbar.Critical(It.IsAny<InvalidOperationException>(), It.IsAny<Dictionary<string, object?>>()));
        }
    }
}