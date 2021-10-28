using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Quartz;

namespace XorusDiscordBot
{
    // ReSharper disable once ClassNeverInstantiated.Global - instantiated by Scheduler
    public class DiscordActionRunner : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var data = context.JobDetail.JobDataMap;
            var id = data.GetIntValue("reminder");
            var reminder = State.Reminders[id];
            await Console.Out.WriteLineAsync("Running job: " + reminder.Name);
            switch (reminder.Action)
            {
                case MessageAction messageAction:
                    await MessageAction(messageAction);
                    break;
                case ChannelTitleAction ctAction:
                    await ChangeTitleAction(ctAction);
                    break;
            }
        }

        private async Task<DiscordChannel> GetChannel(ulong channel)
        {
            // todo: handle channel not existing
            return await State.Discord.GetChannelAsync(channel);
        }

        private async Task MessageAction(MessageAction action)
        {
            string str;
            if (action.Strings.Count == 0) return;
            if (!action.Rotate) str = action.Strings.First();
            else
            {
                var i = action.NextPick % action.Strings.Count; // LastPick could be invalid if list changed
                action.NextPick = i;
                str = action.Strings[i];
                action.NextPick = (i + 1) % action.Strings.Count;
            }

            // todo: handle send error
            await new DiscordMessageBuilder()
                .WithAllowedMention(RoleMention.All)
                .WithAllowedMention(UserMention.All)
                .WithAllowedMention(EveryoneMention.All)
                .WithContent(str)
                .SendAsync(await GetChannel(action.Channel));
            
            await State.Save();
        }

        private async Task ChangeTitleAction(ChannelTitleAction action)
        {
            await (await GetChannel(action.Channel)).ModifyAsync(model => { model.Name = action.Title; });
        }
    }
}