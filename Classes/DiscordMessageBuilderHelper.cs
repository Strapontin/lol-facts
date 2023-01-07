using DSharpPlus;
using DSharpPlus.Entities;
using lol_facts.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lol_facts.Classes
{
    public class DiscordMessageBuilderHelper
    {
        public static DiscordInteractionResponseBuilder BuildMessageFacts(string tag, int index)
        {
            string message = FactsManager.GetFactMessage(tag, ref index);

            // Buttons
            List<DiscordComponent> discordComponents = new()
            {
                new DiscordButtonComponent(ButtonStyle.Primary, JsonConvert.SerializeObject(new ButtonData(ButtonActionEnum.Left, tag, index)), "←"),
                new DiscordButtonComponent(ButtonStyle.Primary, JsonConvert.SerializeObject(new ButtonData(ButtonActionEnum.Right, tag, index)), "→"),
            };

            // Message
            var content = new DiscordInteractionResponseBuilder()
                .WithContent(message)
                .AddComponents(discordComponents);

            return content;
        }
    }
}
