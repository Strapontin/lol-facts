using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using lol_facts.Classes;
using lol_facts.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lol_facts.Commands
{
    public class SlashCommands : ApplicationCommandModule
    {
        public override async Task<bool> BeforeSlashExecutionAsync(InteractionContext ctx)
        {
            bool result = FactsManager.CanExecuteSlashCommand(ctx);

            if (!result)
            {
                await ctx.CreateResponseAsync("Ce channel n'est pas autorisé pour le bot !", true);
            }

            return result;
        }

        [SlashCommand("fact", "Affiche un fact sur l'univers de League of Legends !")]
        public static async Task FactCommand(InteractionContext ctx,
            [Option("Tag", "Un tag spécifique à rechercher.")] string tag = null,
            [Option("Index", "Le numéro du fact dans la liste des résultats, aléatoire par défaut. 1 = 1er, 2 = 2ème...")] long index = -1)
        {
            if (!int.TryParse(index.ToString(), out int indexInt))
            {
                await ctx.CreateResponseAsync($"L'entier correspondant à l'index est trop grand ! Valeur max = {int.MaxValue}");
                return;
            }

            var content = DiscordMessageBuilderHelper.BuildMessageFacts(tag, indexInt);

            await ctx.CreateResponseAsync(content);
        }

        [SlashCommand("enable", "Autorise le bot à envoyer des messages dans ce salon (admin only).")]
        public static async Task EnableCommand(InteractionContext ctx)
        {
            if ((ctx.Member.Permissions & DSharpPlus.Permissions.Administrator) == DSharpPlus.Permissions.None)
            {
                await ctx.CreateResponseAsync("Vous n'avez pas les droits pour effectuer cette action.");
                return;
            }

            FileReader.AddEnableChannel(ctx.Channel.Id);

            await ctx.CreateResponseAsync("Salon autorisé. Que les facts commencent !");
        }

        [SlashCommand("disable", "Annule l'autorisation du bot à envoyer des messages dans ce salon (admin only).")]
        public static async Task DisableCommand(InteractionContext ctx)
        {
            if ((ctx.Member.Permissions & DSharpPlus.Permissions.Administrator) == DSharpPlus.Permissions.None)
            {
                await ctx.CreateResponseAsync("Vous n'avez pas les droits pour effectuer cette action.");
                return;
            }

            FileReader.RemoveEnableChannel(ctx.Channel.Id);

            await ctx.CreateResponseAsync("Ce salon n'est plus autorisé :(");
        }

        [SlashCommand("enablechangelogs", "Autorise le bot à envoyer les notes de version dans ce channel (admin only).")]
        public static async Task EnableChangelogsCommand(InteractionContext ctx)
        {
            if ((ctx.Member.Permissions & DSharpPlus.Permissions.Administrator) == DSharpPlus.Permissions.None)
            {
                await ctx.CreateResponseAsync("Vous n'avez pas les droits pour effectuer cette action.");
                return;
            }

            FileReader.AddEnableChangelog(ctx.Channel.Id);

            await ctx.CreateResponseAsync("Changelogs activés dans ce salon.");
        }

        [SlashCommand("disablechangelogs", "Annule l'autorisation du bot à envoyer les notes de version dans ce channel (admin only).")]
        public static async Task DisableChangelogCommand(InteractionContext ctx)
        {
            if ((ctx.Member.Permissions & DSharpPlus.Permissions.Administrator) == DSharpPlus.Permissions.None)
            {
                await ctx.CreateResponseAsync("Vous n'avez pas les droits pour effectuer cette action.");
                return;
            }

            FileReader.RemoveEnableChangelog(ctx.Channel.Id);

            await ctx.CreateResponseAsync("Changelogs désactivés dans ce salon.");
        }

        //[Command("stat")]
        //[Hidden()]
        //public async Task StatCommand(CommandContext ctx)
        //{
        //    // Sends the stats only if the channel is private
        //    if (!ctx.Channel.IsPrivate)
        //        return;

        //    // --Afficher le nombre d'appels par tag avec le nombre de personne ayant fait l'appel
        //    // Si un argument est envoyé, donner les détails pour ce tag. Ex : !stat renata => Une ligne par personne avec le nombre de fois que l'appel a été fait


        //    // Gets the data from the file
        //    var stats = FileReader.ReadAllStats();
        //    var result = stats
        //        .GroupBy(stat => new { stat.Tag, stat.IsCorrectSearch })
        //        .OrderBy(stat => stat.Key.IsCorrectSearch)
        //        .ThenByDescending(stat => stat.Sum(s => s.Count)).ToList();

        //    string message = "Statistiques de recherche de fact :\n" + string.Join("\n", result.Select(res => $"**{res.Key.Tag}** {(res.Key.IsCorrectSearch ? "existe" : "n'exite pas")} et a été cherché {res.Sum(s => s.Count)} fois par {res.Count()} personne(s) différente(s)."));

        //    if (message.Length > 2000)
        //        message = message.Remove(2000);

        //    using (var fs = new FileStream(Constant.FactsSearchLoggedFilePath, FileMode.Open, FileAccess.Read))
        //    {
        //        var msg = await new DiscordMessageBuilder()
        //            .WithContent(message)
        //            .WithFile(fs)
        //            .SendAsync(ctx.Channel);
        //    }
        //}
    }
}
