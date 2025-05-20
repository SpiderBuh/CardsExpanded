using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Keycards;
using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using InventorySystem;
using CommandSystem.Commands.RemoteAdmin.Inventory;
using System.Reflection;

namespace CardsExpanded.Features
{
    public class ExpandedCards
    {
        public readonly List<Dictionary<string, string>> allCards;

        internal readonly string[] CombinedCardArguments = ["Inventory item name", "Containment", "Armory", "Admin", "Permission color", "Primary tint color", "Label", "Label text color", "Card holder name", "Wear level", "Serial number", "Rank detail option"];

        public ExpandedCards(string ConfigFile)
        {
            StreamReader sr = new StreamReader(ConfigFile);
            string json = sr.ReadToEnd();
            sr.Close();
            allCards = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);
        }

        internal List<Dictionary<string, string>> GetJsonFromAccess(KeycardLevels Permissions)
        {
            return allCards.Where(x => x["Containment"] == Permissions.Containment.ToString() && x["Armory"] == Permissions.Armory.ToString() && x["Admin"] == Permissions.Admin.ToString()).ToList();
        }

        public InventorySystem.Items.Keycards.KeycardItem? GetExpandedKeycard(KeycardLevels Permissions, Player Owner = null, int PreferredIndex = 0)
        {
            var options = GetJsonFromAccess(Permissions);
            if (options == null || options.Count == 0) return null;

            Dictionary<string, string> selected;
            if (PreferredIndex == -1) 
                selected = new Dictionary<string, string>(options.RandomItem());
            else
                selected = new Dictionary<string, string>(options.ElementAt(Mathf.Clamp(PreferredIndex, 0, options.Count - 1)));

            ItemType itemType = (ItemType)Enum.Parse(typeof(ItemType), selected["ItemType"]);

            if (!InventoryExtensions.TryGetTemplate(itemType, out InventorySystem.Items.Keycards.KeycardItem keycardTemplate)) return null;

            InventorySystem.Items.Keycards.KeycardItem keycardItem = GameObject.Instantiate(keycardTemplate);

            var details = keycardItem.Details;

            if (selected.ContainsKey("Card holder name"))
            {
                if (Owner != null)
                {
                    selected["Card holder name"] = selected["Card holder name"].Replace("%", Owner.DisplayName);
                }
                else
                {
                    var NtDs = details.Where(x => x is NametagDetail).Select(y => y as NametagDetail);
                    if (NtDs.Count() >= 1)
                    {
                        selected["Card holder name"] = selected["Card holder name"].Replace("%", ((KeycardWordsCollection)typeof(NametagDetail).GetProperty("_fakeNamesPeople", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(NtDs.First())).NextRandomWord());
                    }
                    else
                    {
                        selected["Card holder name"] = selected["Card holder name"].Replace("%", "John Northwood");
                    }
                }
            }

            if (selected.ContainsKey("Serial number"))
            {
                var SDs = details.Where(x => x is SerialNumberDetail).Select(y => y as SerialNumberDetail);
                if (SDs.Count() == 0 || Owner == null)
                {
                    System.Random rng = new();
                    selected["Serial number"] = ((ulong)(rng.NextDouble() * 1.8446744073709552E+19)).ToString();
                }
                else
                {
                    selected["Serial number"] = ((ulong)typeof(SerialNumberDetail).GetMethod("GetNumberForPlayer", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(SDs.First(), [Owner.ReferenceHub])).ToString();
                }
            }

            if (!keycardItem.Customizable) return keycardItem;

            List<string> arglist = [];
            foreach (var key in CombinedCardArguments)
            {
                if (selected.ContainsKey(key))
                    arglist.Add(selected[key]);
            }
            string[] args = arglist.ToArray();

            int num = 0;
            for (int i = 0; i < details.Length; i++)
            {
                if (details[i] is ICustomizableDetail customizableDetail)
                {
                    var currargs = new ArraySegment<string>(args, num, customizableDetail.CommandArguments.Length);
                    customizableDetail.ParseArguments(currargs);
                    num += customizableDetail.CommandArguments.Length;
                }
            }

            return keycardItem;
        }
    }
}
