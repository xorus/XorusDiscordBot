using System.Collections.Generic;

namespace XorusDiscordBot
{
    public abstract class Action
    {
    }

    public class MessageAction : Action
    {
        public ulong Channel { get; set; }
        public bool Rotate { get; set; }
        public List<string> Strings { get; set; } = new();
        public int NextPick { get; set; }
    }

    public class ChannelTitleAction : Action
    {
        public ulong Channel { get; set; } = 0;

        public string Title { get; set; } = "";
    }

    public class Reminder
    {
        public string Name { get; set; } = "";
        public string Cron { get; set; } = "0 0 0 * * *";
        public ulong Guild { get; set; } = 0;
        public bool InNext { get; set; } = false;
        public Action? Action { get; set; }

        public long? OffsetSeconds { get; set; }
    }
}