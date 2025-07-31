using Interactables.Interobjects.DoorUtils;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.Pickups;
using LabApi.Events.Arguments.Scp914Events;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using Scp914;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils.Networking;
using KeycardItem = LabApi.Features.Wrappers.KeycardItem;

namespace CardsExpanded.Features
{
    public class UpgradeEvents : CustomEventsHandler
    {
        public KeycardLevels? UpgradeCardFromLevels(Scp914KnobSetting setting, KeycardLevels levels)
        {
            KeycardLevels ResultingLevels = levels;
            int accessPower = levels.Containment + levels.Armory + levels.Admin;
            int gambling = UnityEngine.Random.Range(0, 2);
            switch (setting)
            {
                case Scp914KnobSetting.VeryFine:
                    if (gambling == 0)
                        return levels;

                    if (accessPower == 9)
                        return null;

                    if (gambling == 1)
                    {
                        ResultingLevels = upgradeExpandedCardFine(levels);
                    }
                    else
                    {
                        if ((accessPower == 7 || (accessPower == 6 && levels.HighestLevelValue == 3)) && UnityEngine.Random.Range(0, 1) == 1)
                            return null;

                        ResultingLevels = upgradeExpandedCardFine(new(levels.Containment + 1, levels.Armory + 1, levels.Admin + 1));
                    }
                    break;
                case Scp914KnobSetting.Fine:
                    if (accessPower == 9 && gambling == 0)
                        return null;

                    ResultingLevels = upgradeExpandedCardFine(levels);
                    break;
                case Scp914KnobSetting.OneToOne:
                    ResultingLevels = new(levels.Armory, levels.Admin, levels.Containment);
                    break;
                case Scp914KnobSetting.Coarse:
                coarse:
                    if (accessPower == 0)
                        return null;

                    int[] Access = [levels.Containment, levels.Armory, levels.Admin];
                    int highest = 0;

                    int loss = (accessPower + 2) / 3;
                    while (loss > 0)
                    {
                        for (int i = 0; i < 3; i++)
                            if (Access[i] > Access[highest])
                                highest = i;

                        Access[highest]--;
                        loss--;
                    }

                    ResultingLevels = new(Access[0], Access[1], Access[2]);
                    break;
                case Scp914KnobSetting.Rough:
                    if (gambling == 0)
                    {
                        goto coarse;
                    }
                    else
                    {
                        if (accessPower < gambling + 1)
                            return null;

                        accessPower += accessPower / 2;
                        goto coarse;
                    }
            }

            return ResultingLevels;
        }

        private KeycardLevels upgradeExpandedCardFine(KeycardLevels levels) // What a mess...
        {
            int[] Access = [levels.Containment, levels.Armory, levels.Admin];

            int n3 = Access.Count(x => x == 3);
            int n2 = Access.Count(x => x == 2);
            int n1 = Access.Count(x => x == 1);
            int n0 = Access.Count(x => x == 0);

            if ((n3 >= 2) || (n2 == 2 && n3 == 1) || (n2 == 3)) return new(3, 3, 3);

            if (n2 == 2)
            {
                for (int i = 0; i < 3; i++)
                    if (Access[i] == 2) Access[i]++;
                return new(Access[0], Access[1], Access[2]);
            }

            if (n0 == 2)
            {
                if (n1 == 1)
                {
                    for (int i = 0; i < 3; i++)
                        if (Access[i] == 1)
                        {
                            Access[i]++;
                            Access[(i - 1) % 3]++;
                            break;
                        }
                }
                else
                    for (int i = 0; i < 3; i++)
                        if (Access[i] == 0) Access[i]++;

                return new(Access[0], Access[1], Access[2]);
            }

            for (int i = 0; i < 3; i++)
                if (Access[i] == 1 || Access[i] == 2) Access[i]++;

            if (n0 == 3) Access[0]++;

            return new(Access[0], Access[1], Access[2]);
        }

        /*
        public override void OnScp914ProcessingPickup(Scp914ProcessingPickupEventArgs args)
        {
            if (MonoBehaviour.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).FirstOrDefault(x => x.)) { LabApi.Features.Console.Logger.Error("could not find keycard"); return; }

            args.IsAllowed = false;

            var resultLevels = UpgradeCardFromLevels(args.KnobSetting, new(keycard.GetPermissions(null)));
            if (resultLevels == null)
            {
                args.Pickup.Destroy();
                return;
            }

            LabApi.Features.Wrappers.KeycardPickup thing;
            thing.Base.OnCollided +=;
            

            var resultCard = CardsExpandedPlugin.ExpCards.GetExpandedKeycard((KeycardLevels)resultLevels);
            var resultPickup = resultCard.ServerDropItem(true);
            
            resultPickup.Position = args.NewPosition;
        }*/
        public override void OnScp914ProcessingInventoryItem(Scp914ProcessingInventoryItemEventArgs args)
        {
            if (args.Item is not KeycardItem keycard) return;

            args.IsAllowed = false;

            var resultLevelsOrNull = UpgradeCardFromLevels(args.KnobSetting, new(keycard.Base.GetPermissions(null)));
            args.Item.DropItem().Destroy();

            if (resultLevelsOrNull == null) return;

            KeycardLevels resultLevels = (KeycardLevels)resultLevelsOrNull;

            int pIndex = -1;
            if (resultLevels.Containment + resultLevels.Armory + resultLevels.Admin == 9 && UnityEngine.Random.Range(0f, 1f) > 0.02f)
            {
                pIndex = 1; // O5 card more than 98% of the time
            }

            var resultCard = CardsExpandedPlugin.ExpCards.GetExpandedKeycard(resultLevels, args.Player, pIndex);
            args.Player.Inventory.ServerAddItem(resultCard.ItemTypeId, InventorySystem.Items.ItemAddReason.Scp914Upgrade, resultCard.ItemSerial);
        }
    }
}
