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
using static lol_facts.Commands.DiscordChoiceProviders;

namespace lol_facts.Commands
{
    public class TimerSlashCommands : ApplicationCommandModule
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

        [SlashCommand("make-recurring-fact", "Envoi de facts hebdomadaire dans un salon. (admin only)")]
        public async Task MakeRecurringMessageCommand(InteractionContext ctx,
            [Option("Channel", "Channel dans lequel envoyer le message")] DiscordChannel discordChannel,
            [Option("Day", "Jour de la semaine lorsque le message doit être envoyé")][ChoiceProvider(typeof(DaysOfWeekChoiceProvider))] long day,
            [Option("Hour", "Heure à laquel le message doit être envoyé")] long hour)
        {
            if ((ctx.Member.Permissions & Permissions.Administrator) == Permissions.None)
            {
                await ctx.CreateResponseAsync("Vous n'avez pas les droits pour effectuer cette action.");
                return;
            }

            if (hour < 0 || hour > 23)
            {
                var contentError = new DiscordInteractionResponseBuilder().WithContent("L'heure doit être compris entre 0 et 23");

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, contentError);
                return;
            }

            TimerMessageCommand.SetTimerMessage(discordChannel, (int)day, (int)hour);

            var content = new DiscordInteractionResponseBuilder()
                .WithContent($"Commande reçue ! Les facts seront envoyés le {DaysOfWeekChoiceProvider.IntToStringDay(day)} à {hour}h, dans le salon '{discordChannel.Name}'")
                .AsEphemeral();

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, content);
        }

        [SlashCommand("see-recurring-facts", "Récupère un fichier .csv qui contient tous les détails des messages paramétrés. (admin only)")]
        public async Task SeeRecurringMessagesCommand(InteractionContext ctx)
        {
            if ((ctx.Member.Permissions & Permissions.Administrator) == Permissions.None)
            {
                await ctx.CreateResponseAsync("Vous n'avez pas les droits pour effectuer cette action.");
                return;
            }

            var file = FileReader.GetTimerFile();

            var content = new DiscordInteractionResponseBuilder()
                .AddFile(file);

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, content);

            file.Dispose();
            file.Close();
        }

        [SlashCommand("cancel-recurring-facts", "Annule la publication de facts hebdomadaires dans un salon. (admin only)")]
        public async Task CancelRecurringMessageCommand(InteractionContext ctx,
            [Option("Id", "Id de l'objet à annuler")] long id)
        {
            if ((ctx.Member.Permissions & Permissions.Administrator) == Permissions.None)
            {
                await ctx.CreateResponseAsync("Vous n'avez pas les droits pour effectuer cette action.");
                return;
            }

            bool success = TimerMessageCommand.TryCancelTimerFromId((uint)id);

            string contentString = success ? $"Le timer avec l'id {id} bien été annulé" : $"Le timer avec l'id {id} n'a pas été trouvé";
            var content = new DiscordInteractionResponseBuilder().WithContent(contentString);

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, content);
        }
    }
}
