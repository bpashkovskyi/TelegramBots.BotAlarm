using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using TelegramBots.BotAlarm.Domain.Model.ValueObjects;

namespace TelegramBots.BotAlarm.Domain.Model.Entities;

public class DbChat
{
    public DbChat(Chat chat)
    {
        this.TelegramId = chat.Id;
        this.Type = chat.Type;
        this.Title = chat.Title;
        this.Username = chat.Username;
        this.FirstName = chat.FirstName;
        this.LastName = chat.LastName;
        this.Bio = chat.Bio;
        this.Description = chat.Description;
        this.InviteLink = chat.InviteLink;
        this.SlowModeDelay = chat.SlowModeDelay;
        this.LinkedChatId = chat.LinkedChatId;

        this.Settings = new ChatSettings
        {
            BroadcastMessageDuringAlarm = true,
        };

        this.Status = new ChatStatus();
    }

    private DbChat()
    {
        this.Settings = new ChatSettings();
        this.Status = new ChatStatus();
    }

    public int Id { get; private set; }

    public long TelegramId { get; private set; }

    public ChatType Type { get; private set; }

    public ChatSettings Settings { get; private set; }

    public ChatStatus Status { get; private set; }

    public string? Title { get; private set; }

    public string? Username { get; private set; }

    public string? FirstName { get; private set; }

    public string? LastName { get; private set; }

    public string? Bio { get; private set; }

    public string? Description { get; private set; }

    public string? InviteLink { get; private set; }

    public int? SlowModeDelay { get; private set; }

    public long? LinkedChatId { get; private set; }

    public ICollection<AlarmLogMessage> AlarmLogMessages { get; private set; } = new List<AlarmLogMessage>();

    public ICollection<CurfewLogMessage> CurfewLogMessages { get; private set; } = new List<CurfewLogMessage>();
}