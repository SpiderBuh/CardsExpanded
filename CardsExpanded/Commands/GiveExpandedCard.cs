using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabApi.Features.Wrappers;
using Utils;
using InventorySystem;
using Interactables.Interobjects.DoorUtils;

namespace CardsExpanded.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class GiveExpandedCard : ICommand, IUsageProvider
    {
        public string Command => "giveexpandedcard";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Gives the CardsExpanded card for the specified access";

        public string[] Usage { get; } = { /*"player(s)",*/ "Containment", "Armory", "Admin", "(optional) preferred index" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                if (!Player.TryGet(sender, out var plr))
                {
                    response = "Could not get player";
                    return false;
                }
                KeycardLevels perms = new KeycardLevels(int.Parse(arguments.At(0)), int.Parse(arguments.At(1)), int.Parse(arguments.At(2)));
                int preferredIndex = -1;
                if (arguments.Count >= 4)
                {
                    preferredIndex = int.Parse(arguments.At(3));
                }

                var card = CardsExpandedPlugin.ExpCards.GetExpandedKeycard(perms, plr, preferredIndex);

                plr.ReferenceHub.inventory.ServerAddItem(card.ItemTypeId, InventorySystem.Items.ItemAddReason.AdminCommand, card.ItemSerial);
                response = $"Card added successfully!";
                return true;
            }catch (Exception e)
            {
                response = e.ToString();
                return false;
            }


        }
    }
}
