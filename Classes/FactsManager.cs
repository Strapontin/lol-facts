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

        public static string GetFactMessage(string tag, int index)
        {
            string formattedTag = TagsShortcut.ReplaceTagsShortcut(tag);

            index--;

            List<Fact> facts = FileReader.ReadAllFacts()
                .Where(f => string.IsNullOrEmpty(tag) || f.Tags.Select(t => t.FormatTag()).Contains(formattedTag.FormatTag()))
                .ToList();


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
                    Random random = new();
                    index = random.Next(facts.Count);

                    fact = facts[index];
                }

                // Fact header : index
                message = $"Fact {index + 1}/{facts.Count}";

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
    }
}
