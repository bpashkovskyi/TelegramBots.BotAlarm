////namespace TelegramBots.BotAlarm.UnitTests.Business.Services
////{
////    using System.Threading.Tasks;

////    using Microsoft.VisualStudio.TestTools.UnitTesting;

////    using Moq;

////    using TelegramBots.BotAlarm.Business.Services;
////    using TelegramBots.BotAlarm.Domain.Entity;
////    using TelegramBots.BotAlarm.Domain.Enum;
////    using TelegramBots.BotAlarm.Infrastructure.Base;

////    [TestClass]
////    public class AlarmNotificationServiceTest
////    {
////        [TestInitialize]
////        public async Task TestInitialize()
////        {
////            await using (var alarmBotContext = DbContextFactory.Create())
////            {
////                alarmBotContext.Database.EnsureDeleted();
////                alarmBotContext.Database.EnsureCreated();
////            }
////        }

////        [TestMethod]
////        public async Task NotifyAlarmAsync()
////        {
////            await using (var alarmBotContext = DbContextFactory.Create())
////            {
////                alarmBotContext.ServiceLogs.Add(new ServiceLog(ServiceType.Stop));
////                await alarmBotContext.SaveChangesAsync();
////            }

////            var safeTelegramClientMock = new Mock<ISafeTelegramClient>(MockBehavior.Strict);

////            await using (var alarmBotContext = DbContextFactory.Create())
////            {
////                var alarmStatusCheckingService = new AlarmNotificationService(safeTelegramClientMock.Object, alarmBotContext);

////                await alarmStatusCheckingService.NotifyAlarmAsync();
////            }
////        }
////    }
////}