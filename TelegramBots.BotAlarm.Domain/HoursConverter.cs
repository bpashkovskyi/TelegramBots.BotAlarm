namespace TelegramBots.BotAlarm.Domain;

public static class HoursConverter
{
    public static string ConvertHoursToString(int hours)
    {
        if (hours % 10 is 1 && hours % 100 is not 11)
        {
            return "година";
        }

        if (hours % 10 is 2 or 3 or 4 && hours % 100 is not 12 and not 13 and not 14)
        {
            return "години";
        }

        return "годин";
    }
}