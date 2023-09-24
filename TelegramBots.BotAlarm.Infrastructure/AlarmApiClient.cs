using TelegramBots.BotAlarm.Domain;

namespace TelegramBots.BotAlarm.Infrastructure;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Telegram.Bot;

using TelegramBots.BotAlarm.Infrastructure.Base;

public class AlarmApiClient : IAlarmApiClient
{
    private readonly ITelegramBotClient telegramBotClient;

    public AlarmApiClient(ITelegramBotClient telegramBotClient)
    {
        this.telegramBotClient = telegramBotClient;
    }

    public async Task<Dictionary<string, bool>?> GetRegionAlarmsAsync()
    {
        var regionAlarms = await this.GetVadimKlimenkoRegionAlarmsAsync();

        return regionAlarms;
    }

    private async Task<Dictionary<string, bool>?> GetVadimKlimenkoRegionAlarmsAsync()
    {
        try
        {
            var jsonString = await this.DownloadJsonAsync(new Uri("https://vadimklimenko.com/map/statuses.json"));

            var regionAlarms = JObject.Parse(jsonString)["states"]?.Children()
                .Select(region => region as JProperty)
                .ToDictionary(region => region!.Name, region => bool.Parse(region!.Value["enabled"]!.ToString()));

            return regionAlarms;
        }
        catch
        {
            await this.telegramBotClient.SendTextMessageAsync(AppSettings.BoId, "VadimKlimenkoApiClient failed");
            return null;
        }
    }

    private async Task<string> DownloadJsonAsync(Uri url)
    {
        using var httpClient = new HttpClient();
        var httpResponseMessage = await httpClient.GetAsync(url);
        var stringContent = await httpResponseMessage.Content.ReadAsStringAsync();

        return stringContent;
    }
}