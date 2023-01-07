using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using lol_facts.Classes;
using lol_facts.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lol_facts
{
    public static class ButtonInteractions
    {
        public static async Task HandleInteraction(ComponentInteractionCreateEventArgs componentInteractionCreateEventArgs)
        {
            //Logger.LogInfo($"{nameof(HandleInteraction)} : Start");

            var data = JsonConvert.DeserializeObject<ButtonData>(componentInteractionCreateEventArgs.Id);

            switch (data.Action)
            {
                case ButtonActionEnum.Left:
                case ButtonActionEnum.Right:
                    await HandleShowPreviousFact(componentInteractionCreateEventArgs, data);
                    break;
            }

            //Logger.LogInfo($"{nameof(HandleInteraction)} : Start");
        }

        /// <summary>
        /// Handles the selection of roles in the select
        /// </summary>
        /// <param name="componentInteractionCreateEventArgs"></param>
        /// <returns></returns>
        private static async Task HandleShowPreviousFact(ComponentInteractionCreateEventArgs componentInteractionCreateEventArgs, ButtonData buttonData)
        {
            //Logger.LogInfo($"{nameof(HandleCreateRolesMessage)} : Start");

            if (buttonData.Action == ButtonActionEnum.Left && --buttonData.Index <= 0)
            {
                buttonData.Index = int.MaxValue;
            }
            else if (buttonData.Action == ButtonActionEnum.Right && ++buttonData.Index > FactsManager.CountFactsWithTag(buttonData.Tag))
            {
                buttonData.Index = 1;
            }
            
            var content = DiscordMessageBuilderHelper.BuildMessageFacts(buttonData.Tag, buttonData.Index);
            await componentInteractionCreateEventArgs.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, content);

            //Logger.LogInfo($"{nameof(HandleCreateRolesMessage)} : End");
        }
    }
}
