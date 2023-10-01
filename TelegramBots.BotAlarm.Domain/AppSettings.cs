using System.Globalization;

namespace TelegramBots.BotAlarm.Domain;

public static class AppSettings
{
    public const long BoId = 301371111;

    public const long AdminChatId = -1001555787343;

    public static string IfRegion => "Івано-Франківська область";

    public static string AlarmText => $"🔴 <b>{UaLocalTime.ToString("HH:mm", CultureInfo.InvariantCulture)}</b> УВАГА! Повітряна тривога!! 🚨🚨🚨\nПросимо всіх терміново прослідувати в укриття цивільного захисту ‼️";

    public static string ContinueText => $"🟡  <b>{UaLocalTime.ToString("HH:mm", CultureInfo.InvariantCulture)}</b> Повітряна тривога ще триває.\nБудь ласка, залишайтесь в укритті або подалі від вікон ‼️";

    public static string RejectText => $"🟢  <b>{UaLocalTime.ToString("HH:mm", CultureInfo.InvariantCulture)}</b> Відбій тривоги!\nСлідкуйте за подальшими повідомленнями.";

    public static string BlockText => "Можливість писати повідомлення з'явиться після команди \"Відбій\"";

    public static string UnblockText => "Тепер є можливість писати повідомлення.";

    public static string BotStoppedMessage => "Бота переведено у ручний режим";

    public static string BotStartedMessage => "Бота переведено у автоматичний режим";

    public static string CurfewBlockText => "Можливість писати повідомлення з'явиться після закінчення комендантської години.";

    public static string CurfewUnblockText => "Комендантська година закінчилася. Тепер є можливість писати повідомлення.";

    private static DateTime UaLocalTime => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time"));

    public static string AlarmMessageSentText(int alarmLogId) => $"Номер повідомлення про тривогу: {alarmLogId}. Виконайте команду '/deletealarm {alarmLogId}', щоб видалити це повідомлення зі всіх чатів. Виконайте команду '/stop', щоб зупинити автоматичний режим бота.";

    public static string CurfewMessageSentText(int curfewLogId) => $"Номер повідомлення про коменданську годину: {curfewLogId}. Виконайте команду '/deletecurfew {curfewLogId}', щоб видалити це повідомлення зі всіх чатів. Виконайте команду '/stop', щоб зупинити автоматичний режим бота.";
}