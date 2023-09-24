using TelegramBots.BotAlarm.Application.Base;
using TelegramBots.BotAlarm.Domain;
using TelegramBots.BotAlarm.Domain.Model.Entities;
using TelegramBots.BotAlarm.Domain.Model.Enums;
using TelegramBots.BotAlarm.Infrastructure.Base;

namespace TelegramBots.BotAlarm.UnitTests.Domain.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using Rollbar;

[TestClass]
public class AlarmStatusCheckingServiceTest
{
    [TestInitialize]
    public async Task TestInitialize()
    {
        var alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            await alarmBotContext.Database.EnsureDeletedAsync();
            await alarmBotContext.Database.EnsureCreatedAsync();
        }
    }

    [TestMethod]
    public async Task CheckForAlarmAndNotifyShouldNotDoAnythingIfLastServiceLogIsNull()
    {
        var alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            alarmBotContext.ServiceLogs.Add(new ServiceLog(ServiceType.Stop));
            await alarmBotContext.SaveChangesAsync();
        }

        var rollbarMock = new Mock<IRollbar>(MockBehavior.Strict);
        var alarmNotificationServiceMock = new Mock<IAlarmService>(MockBehavior.Strict);
        var alarmApiClientMock = new Mock<IAlarmApiClient>(MockBehavior.Strict);

        alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            var alarmStatusCheckingService = new AlarmStatusCheckingService(rollbarMock.Object, alarmNotificationServiceMock.Object, alarmApiClientMock.Object, alarmBotContext);

            await alarmStatusCheckingService.CheckForAlarmAndNotifyAsync();
        }
    }

    [TestMethod]
    public async Task CheckForAlarmAndNotifyShouldNotifyAlarmIfAlarmIsActiveAndLastAlarmLogIsNull()
    {
        var rollbarMock = new Mock<IRollbar>(MockBehavior.Strict);
        var alarmNotificationServiceMock = new Mock<IAlarmService>(MockBehavior.Strict);
        var alarmApiClientMock = new Mock<IAlarmApiClient>(MockBehavior.Strict);

        alarmApiClientMock.Setup(alarmApiClient => alarmApiClient.GetRegionAlarmsAsync())
            .ReturnsAsync(new Dictionary<string, bool> { { AppSettings.IfRegion, true } });

        alarmNotificationServiceMock.Setup(alarmNotificationService => alarmNotificationService.NotifyAlarmAsync())
            .Returns(Task.FromResult(0));

        var alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            var alarmStatusCheckingService = new AlarmStatusCheckingService(rollbarMock.Object, alarmNotificationServiceMock.Object, alarmApiClientMock.Object, alarmBotContext);

            await alarmStatusCheckingService.CheckForAlarmAndNotifyAsync();

            alarmNotificationServiceMock.Verify(alarmNotificationService => alarmNotificationService.NotifyAlarmAsync());
        }
    }

    [TestMethod]
    public async Task CheckForAlarmAndNotifyShouldNotifyAlarmIfAlarmIsActiveAndLastAlarmLogHasTypeReject()
    {
        var alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            alarmBotContext.AlarmLogs.Add(new AlarmLog(AlarmEventType.Reject));
            await alarmBotContext.SaveChangesAsync();
        }

        var rollbarMock = new Mock<IRollbar>(MockBehavior.Strict);
        var alarmNotificationServiceMock = new Mock<IAlarmService>(MockBehavior.Strict);
        var alarmApiClientMock = new Mock<IAlarmApiClient>(MockBehavior.Strict);

        alarmApiClientMock.Setup(alarmApiClient => alarmApiClient.GetRegionAlarmsAsync())
            .ReturnsAsync(new Dictionary<string, bool> { { AppSettings.IfRegion, true } });

        alarmNotificationServiceMock.Setup(alarmNotificationService => alarmNotificationService.NotifyAlarmAsync())
            .Returns(Task.FromResult(0));

        alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            var alarmStatusCheckingService = new AlarmStatusCheckingService(rollbarMock.Object, alarmNotificationServiceMock.Object, alarmApiClientMock.Object, alarmBotContext);

            await alarmStatusCheckingService.CheckForAlarmAndNotifyAsync();

            alarmNotificationServiceMock.Verify(alarmNotificationService => alarmNotificationService.NotifyAlarmAsync());
        }
    }

    [TestMethod]
    public async Task CheckForAlarmAndNotifyShouldNotifyAlarmIfAlarmIsActiveAndLastAlarmLogHasTypeQuiteTime()
    {
        var alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            alarmBotContext.AlarmLogs.Add(new AlarmLog(AlarmEventType.QuiteTime));
            await alarmBotContext.SaveChangesAsync();
        }

        var rollbarMock = new Mock<IRollbar>(MockBehavior.Strict);
        var alarmNotificationServiceMock = new Mock<IAlarmService>(MockBehavior.Strict);
        var alarmApiClientMock = new Mock<IAlarmApiClient>(MockBehavior.Strict);

        alarmApiClientMock.Setup(alarmApiClient => alarmApiClient.GetRegionAlarmsAsync())
            .ReturnsAsync(new Dictionary<string, bool> { { AppSettings.IfRegion, true } });

        alarmNotificationServiceMock.Setup(alarmNotificationService => alarmNotificationService.NotifyAlarmAsync())
            .Returns(Task.FromResult(0));

        alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            var alarmStatusCheckingService = new AlarmStatusCheckingService(rollbarMock.Object, alarmNotificationServiceMock.Object, alarmApiClientMock.Object, alarmBotContext);

            await alarmStatusCheckingService.CheckForAlarmAndNotifyAsync();

            alarmNotificationServiceMock.Verify(alarmNotificationService => alarmNotificationService.NotifyAlarmAsync());
        }
    }

    [TestMethod]
    public async Task CheckForAlarmAndNotifyShouldLogExceptionIfNotifyAlarmFailed()
    {
        var alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            alarmBotContext.AlarmLogs.Add(new AlarmLog(AlarmEventType.QuiteTime));
            await alarmBotContext.SaveChangesAsync();
        }

        var rollbarMock = new Mock<IRollbar>(MockBehavior.Strict);
        var alarmNotificationServiceMock = new Mock<IAlarmService>(MockBehavior.Strict);
        var alarmApiClientMock = new Mock<IAlarmApiClient>(MockBehavior.Strict);

        rollbarMock.Setup(rollbar => rollbar.Critical(It.IsAny<InvalidOperationException>(), It.IsAny<Dictionary<string, object?>>()))
            .Returns(rollbarMock.Object);

        alarmApiClientMock.Setup(alarmApiClient => alarmApiClient.GetRegionAlarmsAsync())
            .ReturnsAsync(new Dictionary<string, bool> { { AppSettings.IfRegion, true } });

        alarmNotificationServiceMock.Setup(alarmNotificationService => alarmNotificationService.NotifyAlarmAsync())
            .ThrowsAsync(new InvalidOperationException());

        alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            var alarmStatusCheckingService = new AlarmStatusCheckingService(rollbarMock.Object, alarmNotificationServiceMock.Object, alarmApiClientMock.Object, alarmBotContext);

            await alarmStatusCheckingService.CheckForAlarmAndNotifyAsync();

            rollbarMock.Verify(rollbar => rollbar.Critical(It.IsAny<InvalidOperationException>(), It.IsAny<Dictionary<string, object?>>()));
        }
    }

    [TestMethod]
    public async Task CheckForAlarmAndNotifyShouldNotifyContinueIfAlarmIsActiveAndLastAlarmLogWasMoreThanHourAgo()
    {
        var alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            alarmBotContext.AlarmLogs.Add(new AlarmLog(AlarmEventType.Alarm) { DateTime = DateTime.UtcNow.AddHours(-2) });
            await alarmBotContext.SaveChangesAsync();
        }

        var rollbarMock = new Mock<IRollbar>(MockBehavior.Strict);
        var alarmNotificationServiceMock = new Mock<IAlarmService>(MockBehavior.Strict);
        var alarmApiClientMock = new Mock<IAlarmApiClient>(MockBehavior.Strict);

        alarmApiClientMock.Setup(alarmApiClient => alarmApiClient.GetRegionAlarmsAsync())
            .ReturnsAsync(new Dictionary<string, bool> { { AppSettings.IfRegion, true } });

        alarmNotificationServiceMock.Setup(alarmNotificationService => alarmNotificationService.NotifyContinuationAsync())
            .Returns(Task.FromResult(0));

        alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            var alarmStatusCheckingService = new AlarmStatusCheckingService(rollbarMock.Object, alarmNotificationServiceMock.Object, alarmApiClientMock.Object, alarmBotContext);

            await alarmStatusCheckingService.CheckForAlarmAndNotifyAsync();

            alarmNotificationServiceMock.Verify(alarmNotificationService => alarmNotificationService.NotifyContinuationAsync());
        }
    }

    [TestMethod]
    public async Task CheckForAlarmAndNotifyShouldLogExceptionIfNotifyContinuationFailed()
    {
        var alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            alarmBotContext.AlarmLogs.Add(new AlarmLog(AlarmEventType.Alarm) { DateTime = DateTime.UtcNow.AddHours(-2) });
            await alarmBotContext.SaveChangesAsync();
        }

        var rollbarMock = new Mock<IRollbar>(MockBehavior.Strict);
        var alarmNotificationServiceMock = new Mock<IAlarmService>(MockBehavior.Strict);
        var alarmApiClientMock = new Mock<IAlarmApiClient>(MockBehavior.Strict);

        rollbarMock.Setup(rollbar => rollbar.Critical(It.IsAny<InvalidOperationException>(), It.IsAny<Dictionary<string, object?>>()))
            .Returns(rollbarMock.Object);

        alarmApiClientMock.Setup(alarmApiClient => alarmApiClient.GetRegionAlarmsAsync())
            .ReturnsAsync(new Dictionary<string, bool> { { AppSettings.IfRegion, true } });

        alarmNotificationServiceMock.Setup(alarmNotificationService => alarmNotificationService.NotifyContinuationAsync())
            .ThrowsAsync(new InvalidOperationException());

        alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            var alarmStatusCheckingService = new AlarmStatusCheckingService(rollbarMock.Object, alarmNotificationServiceMock.Object, alarmApiClientMock.Object, alarmBotContext);

            await alarmStatusCheckingService.CheckForAlarmAndNotifyAsync();

            rollbarMock.Verify(rollbar => rollbar.Critical(It.IsAny<InvalidOperationException>(), It.IsAny<Dictionary<string, object?>>()));
        }
    }

    [TestMethod]
    public async Task CheckForAlarmAndNotifyShouldNotifyRejectIfAlarmIsNotActiveAndLastAlarmLogIsAlarm()
    {
        var alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            alarmBotContext.AlarmLogs.Add(new AlarmLog(AlarmEventType.Alarm));
            await alarmBotContext.SaveChangesAsync();
        }

        var rollbarMock = new Mock<IRollbar>(MockBehavior.Strict);
        var alarmNotificationServiceMock = new Mock<IAlarmService>(MockBehavior.Strict);
        var alarmApiClientMock = new Mock<IAlarmApiClient>(MockBehavior.Strict);

        alarmApiClientMock.Setup(alarmApiClient => alarmApiClient.GetRegionAlarmsAsync())
            .ReturnsAsync(new Dictionary<string, bool> { { AppSettings.IfRegion, false } });

        alarmNotificationServiceMock.Setup(alarmNotificationService => alarmNotificationService.NotifyRejectAsync())
            .Returns(Task.FromResult(0));

        alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            var alarmStatusCheckingService = new AlarmStatusCheckingService(rollbarMock.Object, alarmNotificationServiceMock.Object, alarmApiClientMock.Object, alarmBotContext);

            await alarmStatusCheckingService.CheckForAlarmAndNotifyAsync();

            alarmNotificationServiceMock.Verify(alarmNotificationService => alarmNotificationService.NotifyRejectAsync());
        }
    }

    [TestMethod]
    public async Task CheckForAlarmAndNotifyShouldNotifyRejectIfAlarmIsNotActiveAndLastAlarmLogIsContinue()
    {
        var alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            alarmBotContext.AlarmLogs.Add(new AlarmLog(AlarmEventType.Continue));
            await alarmBotContext.SaveChangesAsync();
        }

        var rollbarMock = new Mock<IRollbar>(MockBehavior.Strict);
        var alarmNotificationServiceMock = new Mock<IAlarmService>(MockBehavior.Strict);
        var alarmApiClientMock = new Mock<IAlarmApiClient>(MockBehavior.Strict);

        alarmApiClientMock.Setup(alarmApiClient => alarmApiClient.GetRegionAlarmsAsync())
            .ReturnsAsync(new Dictionary<string, bool> { { AppSettings.IfRegion, false } });

        alarmNotificationServiceMock.Setup(alarmNotificationService => alarmNotificationService.NotifyRejectAsync())
            .Returns(Task.FromResult(0));

        alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            var alarmStatusCheckingService = new AlarmStatusCheckingService(rollbarMock.Object, alarmNotificationServiceMock.Object, alarmApiClientMock.Object, alarmBotContext);

            await alarmStatusCheckingService.CheckForAlarmAndNotifyAsync();

            alarmNotificationServiceMock.Verify(alarmNotificationService => alarmNotificationService.NotifyRejectAsync());
        }
    }

    [TestMethod]
    public async Task CheckForAlarmAndNotifyShouldLogExceptionIfNotifyRejectFailed()
    {
        var alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            alarmBotContext.AlarmLogs.Add(new AlarmLog(AlarmEventType.Continue));
            await alarmBotContext.SaveChangesAsync();
        }

        var rollbarMock = new Mock<IRollbar>(MockBehavior.Strict);
        var alarmNotificationServiceMock = new Mock<IAlarmService>(MockBehavior.Strict);
        var alarmApiClientMock = new Mock<IAlarmApiClient>(MockBehavior.Strict);

        rollbarMock.Setup(rollbar => rollbar.Critical(It.IsAny<InvalidOperationException>(), It.IsAny<Dictionary<string, object?>>()))
            .Returns(rollbarMock.Object);

        alarmApiClientMock.Setup(alarmApiClient => alarmApiClient.GetRegionAlarmsAsync())
            .ReturnsAsync(new Dictionary<string, bool> { { AppSettings.IfRegion, false } });

        alarmNotificationServiceMock.Setup(alarmNotificationService => alarmNotificationService.NotifyRejectAsync())
            .ThrowsAsync(new InvalidOperationException());

        alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            var alarmStatusCheckingService = new AlarmStatusCheckingService(rollbarMock.Object, alarmNotificationServiceMock.Object, alarmApiClientMock.Object, alarmBotContext);

            await alarmStatusCheckingService.CheckForAlarmAndNotifyAsync();

            rollbarMock.Verify(rollbar => rollbar.Critical(It.IsAny<InvalidOperationException>(), It.IsAny<Dictionary<string, object?>>()));
        }
    }

    [TestMethod]
    public async Task CheckForAlarmAndNotifyShouldNotifyQuitTimeIfAlarmIsNotActiveAndLastAlarmLogIsRejectAndLastAlarmLogWasMoreThanHourAgo()
    {
        var alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            alarmBotContext.AlarmLogs.Add(new AlarmLog(AlarmEventType.Reject) { DateTime = DateTime.UtcNow.AddHours(-2) });
            await alarmBotContext.SaveChangesAsync();
        }

        var rollbarMock = new Mock<IRollbar>(MockBehavior.Strict);
        var alarmNotificationServiceMock = new Mock<IAlarmService>(MockBehavior.Strict);
        var alarmApiClientMock = new Mock<IAlarmApiClient>(MockBehavior.Strict);

        alarmNotificationServiceMock.Setup(alarmNotificationService => alarmNotificationService.NotifyQuiteTimeAsync(2))
            .Returns(Task.FromResult(0));

        alarmApiClientMock.Setup(alarmApiClient => alarmApiClient.GetRegionAlarmsAsync())
            .ReturnsAsync(new Dictionary<string, bool> { { AppSettings.IfRegion, false } });

        alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            var alarmStatusCheckingService = new AlarmStatusCheckingService(rollbarMock.Object, alarmNotificationServiceMock.Object, alarmApiClientMock.Object, alarmBotContext);

            await alarmStatusCheckingService.CheckForAlarmAndNotifyAsync();

            alarmNotificationServiceMock.Verify(alarmNotificationService => alarmNotificationService.NotifyQuiteTimeAsync(2));
        }
    }

    [TestMethod]
    public async Task CheckForAlarmAndNotifyShouldNotifyQuitTimeIfAlarmIsNotActiveAndLastAlarmLogIsQuietTimeAndLastAlarmLogWasMoreThanHourAgo()
    {
        var alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            alarmBotContext.AlarmLogs.Add(new AlarmLog(AlarmEventType.Reject) { DateTime = DateTime.UtcNow.AddHours(-3) });
            alarmBotContext.AlarmLogs.Add(new AlarmLog(AlarmEventType.QuiteTime) { DateTime = DateTime.UtcNow.AddHours(-2) });
            await alarmBotContext.SaveChangesAsync();
        }

        var rollbarMock = new Mock<IRollbar>(MockBehavior.Strict);
        var alarmNotificationServiceMock = new Mock<IAlarmService>(MockBehavior.Strict);
        var alarmApiClientMock = new Mock<IAlarmApiClient>(MockBehavior.Strict);

        alarmApiClientMock.Setup(alarmApiClient => alarmApiClient.GetRegionAlarmsAsync())
            .ReturnsAsync(new Dictionary<string, bool> { { AppSettings.IfRegion, false } });

        alarmNotificationServiceMock.Setup(alarmNotificationService => alarmNotificationService.NotifyQuiteTimeAsync(3))
            .Returns(Task.FromResult(0));

        alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            var alarmStatusCheckingService = new AlarmStatusCheckingService(rollbarMock.Object, alarmNotificationServiceMock.Object, alarmApiClientMock.Object, alarmBotContext);

            await alarmStatusCheckingService.CheckForAlarmAndNotifyAsync();

            alarmNotificationServiceMock.Verify(alarmNotificationService => alarmNotificationService.NotifyQuiteTimeAsync(3));
        }
    }

    [TestMethod]
    public async Task CheckForAlarmAndNotifyShouldLogExceptionIfNotifyQuitTimeFailed()
    {
        var alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            alarmBotContext.AlarmLogs.Add(new AlarmLog(AlarmEventType.Reject) { DateTime = DateTime.UtcNow.AddHours(-3) });
            alarmBotContext.AlarmLogs.Add(new AlarmLog(AlarmEventType.QuiteTime) { DateTime = DateTime.UtcNow.AddHours(-2) });
            await alarmBotContext.SaveChangesAsync();
        }

        var rollbarMock = new Mock<IRollbar>(MockBehavior.Strict);
        var alarmNotificationServiceMock = new Mock<IAlarmService>(MockBehavior.Strict);
        var alarmApiClientMock = new Mock<IAlarmApiClient>(MockBehavior.Strict);

        rollbarMock.Setup(rollbar => rollbar.Critical(It.IsAny<InvalidOperationException>(), It.IsAny<Dictionary<string, object?>>()))
            .Returns(rollbarMock.Object);

        alarmApiClientMock.Setup(alarmApiClient => alarmApiClient.GetRegionAlarmsAsync())
            .ReturnsAsync(new Dictionary<string, bool> { { AppSettings.IfRegion, false } });

        alarmNotificationServiceMock.Setup(alarmNotificationService => alarmNotificationService.NotifyQuiteTimeAsync(It.IsAny<int>()))
            .ThrowsAsync(new InvalidOperationException());

        alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            var alarmStatusCheckingService = new AlarmStatusCheckingService(rollbarMock.Object, alarmNotificationServiceMock.Object, alarmApiClientMock.Object, alarmBotContext);

            await alarmStatusCheckingService.CheckForAlarmAndNotifyAsync();

            rollbarMock.Verify(rollbar => rollbar.Critical(It.IsAny<InvalidOperationException>(), It.IsAny<Dictionary<string, object?>>()));
        }
    }

    [TestMethod]
    public async Task CheckForAlarmAndNotifyShouldDoNothingIfAlarmIsActiveAndLastAlarmLogIsAlarm()
    {
        var alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            alarmBotContext.AlarmLogs.Add(new AlarmLog(AlarmEventType.Alarm));
            await alarmBotContext.SaveChangesAsync();
        }

        var rollbarMock = new Mock<IRollbar>(MockBehavior.Strict);
        var alarmNotificationServiceMock = new Mock<IAlarmService>(MockBehavior.Strict);
        var alarmApiClientMock = new Mock<IAlarmApiClient>(MockBehavior.Strict);

        alarmApiClientMock.Setup(alarmApiClient => alarmApiClient.GetRegionAlarmsAsync())
            .ReturnsAsync(new Dictionary<string, bool> { { AppSettings.IfRegion, true } });

        alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            var alarmStatusCheckingService = new AlarmStatusCheckingService(rollbarMock.Object, alarmNotificationServiceMock.Object, alarmApiClientMock.Object, alarmBotContext);

            await alarmStatusCheckingService.CheckForAlarmAndNotifyAsync();
        }
    }

    [TestMethod]
    public async Task CheckForAlarmAndNotifyShouldDoNothingIfAlarmIsNotActiveAndLastAlarmLogIsRejectAndLastAlarmLogWasLessThanHourAgo()
    {
        var alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            alarmBotContext.AlarmLogs.Add(new AlarmLog(AlarmEventType.Reject));
            await alarmBotContext.SaveChangesAsync();
        }

        var rollbarMock = new Mock<IRollbar>(MockBehavior.Strict);
        var alarmNotificationServiceMock = new Mock<IAlarmService>(MockBehavior.Strict);
        var alarmApiClientMock = new Mock<IAlarmApiClient>(MockBehavior.Strict);

        alarmApiClientMock.Setup(alarmApiClient => alarmApiClient.GetRegionAlarmsAsync())
            .ReturnsAsync(new Dictionary<string, bool> { { AppSettings.IfRegion, false } });

        alarmBotContext = DbContextFactory.Create();
        await using (alarmBotContext)
        {
            var alarmStatusCheckingService = new AlarmStatusCheckingService(rollbarMock.Object, alarmNotificationServiceMock.Object, alarmApiClientMock.Object, alarmBotContext);

            await alarmStatusCheckingService.CheckForAlarmAndNotifyAsync();
        }
    }
}