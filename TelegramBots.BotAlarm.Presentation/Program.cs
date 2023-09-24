using HealthChecks.UI.Client;

using MediatR;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

using TelegramBots.BotAlarm.Application;
using TelegramBots.BotAlarm.Domain.Base;
using TelegramBots.BotAlarm.Infrastructure;
using TelegramBots.BotAlarm.Infrastructure.Base;
using TelegramBots.BotAlarm.Persistence;
using TelegramBots.Shared.Extensions;

namespace TelegramBots.BotAlarm.Presentation;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Web
        builder.Services.AddControllers().AddNewtonsoftJson();
        builder.Services.AddHostedService<Scheduler>();

        // Application
        builder.Services.AddUpdatesMediator();
        builder.Services.AddMediatR(typeof(Program));
        builder.Services.AddScoped<IAlarmService, AlarmService>();
        builder.Services.AddScoped<IAlarmNotificationService, AlarmNotificationService>();
        builder.Services.AddScoped<ICurfewService, CurfewService>();
        builder.Services.AddScoped<ICurfewNotificationService, CurfewNotificationService>();
        builder.Services.AddScoped<IChatService, ChatService>();
        builder.Services.AddScoped<IBotService, BotService>();

        // Domain
        builder.Services.AddScoped<IAlarmStatusCheckingService, AlarmStatusCheckingService>();
        builder.Services.AddScoped<ICurfewStatusCheckingService, CurfewStatusCheckingService>();

        // Persistence
        builder.Services.AddDbContext<AlarmBotContext>(options => options.UseSqlServer(
            builder.Configuration.GetConnectionString("AlarmBotConnection"),
            sqlServerDbContextOptionsBuilder => sqlServerDbContextOptionsBuilder.MigrationsHistoryTable("__MigrationsHistory", "alarm")));

        // Infrastructure
        builder.Services.AddScoped<IAlarmApiClient, AlarmApiClient>();
        builder.Services.AddScoped<ISafeTelegramClient, SafeTelegramClient>();

        builder.Services.AddRollbar(builder.Configuration);
        builder.Services.AddTelegramClient(builder.Configuration);

        builder.Services.AddHealthChecks(builder.Configuration)
            .AddDbContextCheck<AlarmBotContext>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapControllers());

        app.UseHealthChecks(
            "/healthchecks",
            new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            });

        app.Run();
    }
}