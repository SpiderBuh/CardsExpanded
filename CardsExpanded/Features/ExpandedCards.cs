using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Keycards;
using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsExpanded.Features
{
    public static class ExpandedCards
    {
        private static List<Dictionary<string, string>> allCards = null;

        private static void loadCards()
        {
            ItemType
        }

        public static InventorySystem.Items.Keycards.KeycardItem? GetExpandedKeycard(KeycardLevels Permissions, Player Owner = null)
        {
            if (allCards == null) loadCards();

            if (!itemType.TryGetTemplate<InventorySystem.Items.Keycards.KeycardItem>(out var item))
            {
                throw new ArgumentException("Template for itemType not found");
            }

            int num = 0;
            DetailBase[] details = item.Details;
            for (int i = 0; i < details.Length; i++)
            {
                if (details[i] is ICustomizableDetail customizableDetail)
                {
                    customizableDetail.SetArguments(new ArraySegment<object>(args, num, customizableDetail.CustomizablePropertiesAmount));
                    num += customizableDetail.CustomizablePropertiesAmount;
                }
            }
        }
    }
}
