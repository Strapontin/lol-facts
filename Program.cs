using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using lol_facts.Classes;
using lol_facts.Commands;
using lol_facts.Entities;
using lol_facts.IO;
using System;
using System.Threading.Tasks;

namespace lol_facts
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        internal static async Task MainAsync()
        {
            FileReader.CreateFilesIfNotExist();

            var discord = new DiscordClient(new DiscordConfiguration()
            {
#if DEBUG
                Token = Environment.GetEnvironmentVariable("DISCORD_KEY_LOL_FACTS_DEBUG"),
#else
                Token = Environment.GetEnvironmentVariable("DISCORD_KEY_LOL_FACTS"),
#endif
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged,
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
            });

            var slash = discord.UseSlashCommands();
            slash.RegisterCommands<EmptyGlobalCommandToAvoidFamousDuplicateSlashCommandsBug>();
            slash.RegisterCommands<SlashCommands>();
            slash.RegisterCommands<TimerSlashCommands>();

            discord.UseInteractivity(new InteractivityConfiguration()
            {
                PollBehaviour = DSharpPlus.Interactivity.Enums.PollBehaviour.KeepEmojis,
            });

            // On button clicked
            discord.ComponentInteractionCreated += async (discordClient, componentInteractionCreateEventArgs) =>
            {
                await ButtonInteractions.HandleInteraction(componentInteractionCreateEventArgs);
            };

            discord.Ready += async (discordClient, componentInteractionCreateEventArgs) =>
            {
                // On discord ready, send a changelog to all channels that ask for it if we're in release 
#if DEBUG
                return;
#endif
#pragma warning disable CS0162 // Unreachable code detected
                string changelogContent = FileReader.ReadLastAvailableChangelog();
#pragma warning restore CS0162 // Unreachable code detected

                if (!string.IsNullOrEmpty(changelogContent))
                {
                    foreach (var channel in FileReader.ReadAllEnabledChangelogChannels())
                    {
                        DiscordChannel discordChannel = await discordClient.GetChannelAsync(ulong.Parse(channel));


                        await discordClient.SendMessageAsync(discordChannel, changelogContent);
                    }

                    FileReader.ArchiveChangelogs();
                }
            };

            FileReader.GenerateFactFile();

            await discord.ConnectAsync();

            // If the bot shut downed and restarted, it needs the timers back
            discord.GuildAvailable += (s, e) =>
            {
                TimerMessageCommand.RestartTimersOnAppStarting(discord);
                return Task.CompletedTask;
            };

            await Task.Delay(-1);
        }
    }
}