using Microsoft.EntityFrameworkCore;

using TelegramBots.BotAlarm.Domain.Model.Entities;

namespace TelegramBots.BotAlarm.Persistence;

public class AlarmBotContext : DbContext
{
    public AlarmBotContext(DbContextOptions<AlarmBotContext> options)
        : base(options)
    {
    }

    public DbSet<DbChat> Chats { get; set; } = null!;

    public DbSet<AlarmLog> AlarmLogs { get; set; } = null!;

    public DbSet<AlarmLogMessage> AlarmLogMessages { get; set; } = null!;

    public DbSet<CurfewLog> CurfewLogs { get; set; } = null!;

    public DbSet<CurfewLogMessage> CurfewLogMessages { get; set; } = null!;

    public DbSet<ServiceLog> ServiceLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("alarm");

        modelBuilder.Entity<DbChat>()
            .HasIndex(chat => new { chat.TelegramId })
            .IsUnique();

        modelBuilder.Entity<DbChat>()
            .OwnsOne(chat => chat.Settings);

        modelBuilder.Entity<DbChat>()
            .OwnsOne(chat => chat.Status);

        modelBuilder.Entity<DbChat>()
            .HasMany(chat => chat.AlarmLogMessages)
            .WithOne(alarmLogMessage => alarmLogMessage.Chat)
            .HasForeignKey(alarmLogMessage => alarmLogMessage.ChatId);

        modelBuilder.Entity<DbChat>()
            .HasMany(chat => chat.CurfewLogMessages)
            .WithOne(curfewLogMessage => curfewLogMessage.Chat)
            .HasForeignKey(curfewLogMessage => curfewLogMessage.ChatId);

        modelBuilder.Entity<CurfewLog>()
            .HasMany(curfewLog => curfewLog.CurfewLogMessages)
            .WithOne(curfewLogMessage => curfewLogMessage.CurfewLog)
            .HasForeignKey(curfewLogMessage => curfewLogMessage.CurfewLogId);
    }
}