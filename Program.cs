using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
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
                Token = Environment.GetEnvironmentVariable("DISCORD_KEY"),
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged,
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
            });

            // Allows us to use modules in Command folder
            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "!" }
            });
            //commands.RegisterCommands<GeneralModule>();
            commands.RegisterCommands<FactModule>();
            commands.SetHelpFormatter<Help>();

            discord.UseInteractivity(new InteractivityConfiguration()
            {
                PollBehaviour = DSharpPlus.Interactivity.Enums.PollBehaviour.KeepEmojis,
            });



            // On button clicked
            discord.ComponentInteractionCreated += async (discordClient, componentInteractionCreateEventArgs) =>
            {
                if (componentInteractionCreateEventArgs.Id.Contains("my_very_cool_button"))
                {
                    // Renvoie un message lié à celui possédant les boutons
                    await componentInteractionCreateEventArgs.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Cool"));
                }
                else if (componentInteractionCreateEventArgs.Id == "1_top")
                {
                    await componentInteractionCreateEventArgs.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent("No more buttons for you >:)"));
                }
                else
                {
                    await componentInteractionCreateEventArgs.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                }
            };

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
