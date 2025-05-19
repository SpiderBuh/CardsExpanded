using CommandSystem;
using InventorySystem.Items.Keycards;
using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CardsExpanded.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ReadCardInfo : ICommand
    {
        public string Command => "readcardinfo";

        public string[] Aliases => [];

        public string Description => "Reads the custom information of the card currently being held";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player plr;
            if (!Player.TryGet(sender, out plr))
            {
                response = "Could not get player";
                return false;
            }

            if (plr.CurrentItem == null || !(plr.CurrentItem.Base is InventorySystem.Items.Keycards.KeycardItem card))
            {
                response = "You are not holding a keycard";
                return false;
            }

            response = "Stuff:\n";

            var details = card.Details;
            foreach (DetailBase detailBase in details)
            {
                try
                {
                    if (detailBase is PredefinedTintDetail tint)
                    {

                        response += detailBase.GetType().Name + "\t";
                        response += ((Color)typeof(PredefinedTintDetail).GetField("_color", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(tint)).ToHex();
                        response += "\n";
                    } else if (detailBase is CustomWearDetail wear)
                    {
                        response += detailBase.GetType().Name + "\t";
                        response += (int)typeof(PredefinedWearDetail).GetField("_wearLevel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(wear);
                        response += "\n";
                    }

                }
                catch (Exception ex)
                {
                    response += "Something failed at: " + detailBase.GetType().Name + ".\n" + ex.Message;
                    return false;
                }
            }



            return true;
        }
    }
}
