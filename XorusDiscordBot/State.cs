using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace XorusDiscordBot
{
    public class ConfigurationFile
    {
        public string Token { get; set; } = "";
        public ulong RegisterTo { get; set; } = 0;
        public List<Reminder> Reminders { get; set; } = new();
    }

    public static class State
    {
        public static readonly Dictionary<int, Reminder> Reminders = new();
        public static DiscordClient Discord { get; set; } = null!;

        private static int _lastIndex = 0;

        public static ConfigurationFile ConfigurationFile = null!;

        private static void Add(Reminder reminder)
        {
            Reminders.Add(_lastIndex++, reminder);
        }

        private static void Populate()
        {
            ConfigurationFile = new ConfigurationFile
            {
                Token = "DiscordBotToken"
            };

            Add(new Reminder
            {
                Name = "Example",
                Cron = "*/15 * * * * ?",
                Action = new MessageAction
                {
                    Channel = 123456789,
                    Rotate = true,
                    Strings = new List<string>
                    {
                        "role mention example <@&role_id>",
                        "user mention example <@user_id>",
                    }
                }
            });

            // Add(new Reminder("15,45,55 * * * * ?",
            //     new ChannelTitleAction(804350874435846144, "bot")));
            Save().Wait();
        }

        private static readonly INamingConvention NamingConvention = UnderscoredNamingConvention.Instance;

        private static readonly ISerializer Serializer =
            new SerializerBuilder()
                .WithNamingConvention(NamingConvention)
                .WithTagMapping("!channel_title", typeof(ChannelTitleAction))
                .WithTagMapping("!message", typeof(MessageAction))
                .Build();

        public static async Task Save()
        {
            ConfigurationFile.Reminders = Reminders.Select(keyValuePair => keyValuePair.Value).ToList();
            var yaml = Serializer.Serialize(ConfigurationFile);
            await File.WriteAllTextAsync("config.yaml", yaml);
        }

        public static async Task Load()
        {
            if (File.Exists("config.yaml"))
            {
                var lines = await File.ReadAllTextAsync("config.yaml");
                var deserializer = new DeserializerBuilder()
                    .WithTagMapping("!channel_title", typeof(ChannelTitleAction))
                    .WithTagMapping("!message", typeof(MessageAction))
                    .WithNamingConvention(NamingConvention)
                    .Build();
                ConfigurationFile = deserializer.Deserialize<ConfigurationFile>(lines);
                Reminders.Clear();
                for (var index = 0; index < ConfigurationFile.Reminders.Count; index++)
                {
                    Reminders.Add(index, ConfigurationFile.Reminders[index]);
                }

                OnLoad?.Invoke(null, EventArgs.Empty);
                await Save();
            }
            else
            {
                Populate();
            }
        }

        public static event EventHandler? OnLoad;
    }
}