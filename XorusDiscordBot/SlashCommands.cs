using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Quartz;

namespace XorusDiscordBot
{
    public class SlashCommands : SlashCommandModule
    {
        [SlashCommand("next", "when is the next planned thing?")]
        public async Task TestCommand(InteractionContext ctx)
        {
            var messageText = "Ding, ding!";
            string reminders = "";

            foreach (var (key, reminder) in State.Reminders)
            {
                if (reminder.InNext && reminder.Guild == ctx.Guild.Id)
                {
                    var c = new CronExpression(reminder.Cron);
                    var next = (c.GetTimeAfter(DateTimeOffset.Now).Value.AddSeconds(reminder.OffsetSeconds ?? 0))
                        .ToUnixTimeSeconds();
                    // https://discord.com/developers/docs/reference#message-formatting-timestamp-styles
                    reminders += String.Format("\n{0} : <t:{1}> (<t:{1}:R>)", reminder.Name, next);
                }
            }

            if (reminders.Length == 0) reminders = "\nNothing is planned at the moment.";
            messageText += reminders;

            await ctx.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent(messageText)
            );
        }
    }
}