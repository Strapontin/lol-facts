using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using lol_facts.Constants;
using lol_facts.Entities;
using lol_facts.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lol_facts.Commands
{
    class FactModule : BaseCommandModule
    {
        /// <summary>
        /// Executes before any other command
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public async override Task BeforeExecutionAsync(CommandContext ctx)
        {
            List<string> authorizedCommands = new List<string>()
            {
                "enable", 
            };

            // If the channel isn't enabled, it isn't a private message and the command isn't supposed to authorize it, cancel the command
            if (!FileReader.IsChannelEnabled(ctx.Channel.Id) &&
                !ctx.Channel.IsPrivate &&
                !authorizedCommands.Contains(ctx.Command.Name))
            {
                throw new Exception();
            }

            await Task.CompletedTask;
        }

        [Command("fact")]
        [Description("Gives a fact")]
        public async Task FactCommand(CommandContext ctx, string tag = null, int index = -1)
        {
            string formattedTag = TagsShortcut.ReplaceTagsShortcut(tag);

            // Select all facts with specific tag
            var facts = FileReader.ReadAllFacts()
                .Where(f => string.IsNullOrEmpty(tag) || f.Tags.Select(t => t.FormatTag()).Contains(formattedTag.FormatTag()))
                .ToList();

            Fact fact;
            bool isCorrectSearch;

            string message;
            if (facts.Count == 0)
            {
                isCorrectSearch = false;
                message = $"Aucun fact avec le tag {tag} n'a été trouvé";
            }
            else
            {
                isCorrectSearch = true;

                // Select the fact depending on the index
                if (index > -1 && index < facts.Count)
                {
                    fact = facts[index];
                }
                else if (index > facts.Count)
                {
                    fact = facts.Last();
                    index = facts.Count - 1;
                }
                else
                {
                    Random random = new();
                    index = random.Next(facts.Count);

                    fact = facts[index];
                }

                message = $"Fact {index + 1}/{facts.Count} sur {tag}\n{fact.Text}";
            }

            string username;
            string mention;
            // If the request was made in a private message, the data is different
            if (ctx.Channel.IsPrivate)
            {
                username = ctx.User.Username;
                mention = ctx.User.Mention.Replace("@", "@!");
            }
            else
            {
                username = ctx.Member.Username;
                mention = ctx.Member.Mention;
            }

            FileReader.AddIncorrectSearch(username, mention, tag, isCorrectSearch);

            await ctx.RespondAsync(message);
        }

        [Command("enable")]
        [Description("Enables this channel for this bot. Can only be executed by a user with Administrator permissions")]
        public async Task EnableCommand(CommandContext ctx)
        {
            if ((ctx.Member.Permissions & DSharpPlus.Permissions.Administrator) == DSharpPlus.Permissions.None)
            {
                await ctx.RespondAsync("Vous n'avez pas les droits pour effectuer cette action");
                return;
            }

            FileReader.AddEnableChannel(ctx.Channel.Id);

            await ctx.RespondAsync("Salon autorisé. Que les facts commencent !");
        }

        [Command("disable")]
        [Description("Disables this channel for this bot. Can only be executed by a user with Administrator permissions")]
        public async Task DisableCommand(CommandContext ctx)
        {
            if ((ctx.Member.Permissions & DSharpPlus.Permissions.Administrator) == DSharpPlus.Permissions.None)
            {
                await ctx.RespondAsync("Vous n'avez pas les droits pour effectuer cette action");
                return;
            }

            FileReader.RemoveEnableChannel(ctx.Channel.Id);

            await ctx.RespondAsync("Ce salon n'est plus autorisé :(");
        }

        [Command("stat")]
        [Hidden()]
        public async Task StatCommand(CommandContext ctx)
        {
            // Sends the stats only if the channel is private
            if (!ctx.Channel.IsPrivate)
                return;

            // --Afficher le nombre d'appels par tag avec le nombre de personne ayant fait l'appel
            // Si un argument est envoyé, donner les détails pour ce tag. Ex : !stat renata => Une ligne par personne avec le nombre de fois que l'appel a été fait


            // Gets the data from the file
            var stats = FileReader.ReadAllStats();
            var result = stats
                .GroupBy(stat => new { stat.Tag, stat.IsCorrectSearch })
                .OrderBy(stat => stat.Key.IsCorrectSearch)
                .ThenByDescending(stat => stat.Sum(s => s.Count)).ToList();

            string message = "Statistiques de recherche de fact :\n" + string.Join("\n", result.Select(res => $"**{res.Key.Tag}** {(res.Key.IsCorrectSearch ? "existe" : "n'exite pas")} et a été cherché {res.Sum(s => s.Count)} fois par {res.Count()} personne(s) différente(s)."));

            if (message.Length > 2000)
                message = message.Remove(2000);

            using (var fs = new FileStream(Constant.FactsWithUnknownTagFilePath, FileMode.Open, FileAccess.Read))
            {
                var msg = await new DiscordMessageBuilder()
                    .WithContent(message)
                    .WithFile(fs)
                    .SendAsync(ctx.Channel);
            }
        }

        [Command("stat")]
        [Hidden()]
        public async Task StatCommand(CommandContext ctx, string tag)
        {
            // Sends the stats only if the channel is private
            if (!ctx.Channel.IsPrivate)
                return;

            // Gets the data from the file
            var stats = FileReader.ReadAllStats();
        }
    }
}
