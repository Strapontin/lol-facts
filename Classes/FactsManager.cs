using DSharpPlus.SlashCommands;
using lol_facts.Constants;
using lol_facts.Entities;
using lol_facts.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lol_facts.Classes
{
    public static class FactsManager
    {
        public static bool CanExecuteSlashCommand(InteractionContext ctx)
        {
            List<string> authorizedCommands = new()
            {
                "enable",
                "enablechangelogs",
            };

            // If the channel isn't enabled && it isn't a private message && the command isn't supposed to authorize it, cancel the command
            if (!FileReader.IsChannelEnabled(ctx.Channel.Id) &&
                !ctx.Channel.IsPrivate &&
                !authorizedCommands.Contains(ctx.CommandName))
            {
                return false;
            }

            return true;
        }

        private static List<Fact> GetFactsWithTag(string tag)
        {
            string formattedTag = TagsShortcut.ReplaceTagsShortcut(tag).FormatTag();

            return FileReader.ReadAllFacts()
                .Where(f => string.IsNullOrEmpty(formattedTag) || f.Tags.Select(t => t.FormatTag()).Contains(formattedTag))
                .ToList();
        }

        public static string GetFactMessage(string tag, ref int index)
        {
            index--;

            List<Fact> facts = GetFactsWithTag(tag);


            Fact fact;

            string message;
            if (facts.Count == 0)
            {
                message = $"Aucun fact avec le tag {tag} n'a été trouvé";
            }
            else
            {
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
                    index = new Random().Next(facts.Count);
                    fact = facts[index];
                }

                // Fact header : index
                message = $"Fact {++index}/{facts.Count}";

                // Shows the tag selected if there is one
                if (!string.IsNullOrEmpty(tag))
                {
                    message += $", tag '{tag}'";
                }

                // Shows all tags on this fact
                message += $" ({string.Join(", ", fact.Tags)})";

                string factTextFormatted = fact.Text
                    .Replace("\"\"", "\"")
                    .Replace("\\n", "\n")
                    .Replace("\\t", "\t");

                if (factTextFormatted.StartsWith('"'))
                {
                    factTextFormatted = factTextFormatted.Substring(1, factTextFormatted.Length - 2);
                }

                message += $"\n\n{factTextFormatted}";
            }

            return message;
        }

        public static int CountFactsWithTag(string tag)
        {
            return GetFactsWithTag(tag).Count;
        }
    }
}
