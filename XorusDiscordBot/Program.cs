// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Specialized;
using System.Threading;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using Quartz;
using XorusDiscordBot;

Console.WriteLine("Hey.");

var scheduler = await SchedulerBuilder.Create(new NameValueCollection())
    .UseDefaultThreadPool(x => x.MaxConcurrency = 1)
    .BuildScheduler();

await scheduler.Start();

async void OnStateOnOnLoad(object? sender, EventArgs eventArgs)
{
    await scheduler.Clear();
    foreach (var (key, reminder) in State.Reminders)
    {
        var job = JobBuilder.Create<DiscordActionRunner>()
            .WithIdentity("reminder" + key, "reminders")
            .UsingJobData("name", reminder.Name)
            .UsingJobData("reminder", key)
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity("trigger" + key, "reminders")
            .WithCronSchedule(reminder.Cron)
            .Build();

        await scheduler.ScheduleJob(job, trigger);
        Console.Out.WriteLine("Register job " + job + " " + trigger);
    }
}

State.OnLoad += OnStateOnOnLoad;

await State.Load();

State.Discord = new DiscordClient(new DiscordConfiguration()
{
    Token = State.ConfigurationFile.Token,
    TokenType = TokenType.Bot,
    Intents = DiscordIntents.AllUnprivileged
});

// https://discord.com/oauth2/authorize?client_id=____&scope=bot&permissions=2147880016

var slash = State.Discord.UseSlashCommands();
slash.RegisterCommands<SlashCommands>(State.ConfigurationFile.RegisterTo);
await State.Discord.ConnectAsync();

AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
{
    scheduler.Shutdown().Wait();
    State.Discord.DisconnectAsync().Wait();
};
Thread.Sleep(Timeout.Infinite);
