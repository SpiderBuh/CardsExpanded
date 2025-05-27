using Interactables.Interobjects.DoorUtils;
using InventorySystem;
using InventorySystem.Items.Pickups;
using static LabApi.Features.Console.Logger;
using LabApi.Features.Interfaces;
using LabApi.Features.Wrappers;
using Scp914;
using System;
using System.Linq;
using UnityEngine;

namespace CardsExpanded.Features
{
    public class ExpandedCardProcessor : IScp914ItemProcessor
    {
        public bool UsePickupMethodOnly => true;

        public Scp914Result UpgradeItem(Scp914KnobSetting setting, Item item)
        {
            throw new NotImplementedException();
        }

        public Scp914Result UpgradePickup(Scp914KnobSetting setting, Pickup pickup)
        {
            return new(pickup.Base);
        }
    }
    //    public bool UsePickupMethodOnly => false;
    //    private InventorySystem.Items.Keycards.KeycardItem source = null;
    //    private InventorySystem.Items.Keycards.KeycardPickup sourcePickup = null;
    //    private void clearSourceItems() { source = null; sourcePickup = null; }

    //    public Scp914Result UpgradeItem(Scp914KnobSetting setting, Item item)
    //    {
    //        Info($"Upgrading item {item.Type}");
    //        if (item is LabApi.Features.Wrappers.KeycardItem keycard)
    //        {
    //            source = keycard.Base;
    //            //sourcePickup = (InventorySystem.Items.Keycards.KeycardPickup)source.PickupDropModel;
    //            var resultItem = UpgradeCardFromLevels(setting, new(source.GetPermissions(null)));
    //            clearSourceItems();
    //            return new(source, resultItem.Base, resultItem.Base.PickupDropModel);
    //        }
    //            Warn("Returning without proper result");
    //        return default;
    //    }

    //    public Scp914Result UpgradePickup(Scp914KnobSetting setting, Pickup pickup)
    //    {
    //        try
    //        {
    //            Info($"Upgrading pickup {pickup.Type}");
    //            if (pickup is LabApi.Features.Wrappers.KeycardPickup keycard)
    //            {
    //                sourcePickup = keycard.Base;
    //                source = keycard.GameObject.GetComponent<InventorySystem.Items.Keycards.KeycardItem>();
    //                Info("Source item obtained");
    //                var resultItem = UpgradeCardFromLevels(setting, new(source.GetPermissions(null)));

    //                var psi = new PickupSyncInfo(resultItem.Base.ItemTypeId, resultItem.Weight, resultItem.Base.ItemSerial);
    //                var resultPos = sourcePickup.Position + Scp914Controller.MoveVector;
    //                Info("Creating pickup");
    //                var resultPickup = InventoryExtensions.ServerCreatePickup(resultItem.Base, psi, resultPos);
    //                sourcePickup.DestroySelf();
    //                clearSourceItems();
    //                Info("Returning Scp914Result");
    //                return new(sourcePickup, resultItem.Base, resultPickup);
    //            }
    //            Warn("Returning without proper result");
    //            return new(sourcePickup);
    //        }
    //        catch (Exception e)
    //        {
    //            Error(e);
    //            return default;
    //        }
    //    }
    //    private Scp914Result destroyedResult()
    //    {
    //        try
    //        {
    //            Info("Item destroyed result");
    //            sourcePickup.DestroySelf();
    //            return new(sourcePickup);
    //        }
    //        catch (Exception e)
    //        {
    //            Error(e);
    //            return default;
    //        }
    //    }
    //    public KeycardItem UpgradeCardFromLevels(Scp914KnobSetting setting, KeycardLevels levels, Player owner = null)
    //    {
    //        KeycardLevels ResultingLevels = levels;
    //        int accessPower = levels.Containment + levels.Armory + levels.Admin;
    //        int gambling = UnityEngine.Random.Range(0, 2);
    //        switch (setting)
    //        {
    //            case Scp914KnobSetting.VeryFine:
    //                if (gambling == 0)
    //                    return new(source, source, sourcePickup);

    //                if (accessPower == 9)
    //                    return destroyedResult();

    //                if (gambling == 1)
    //                {
    //                    ResultingLevels = upgradeExpandedCardFine(levels);
    //                }
    //                else
    //                {
    //                    if ((accessPower == 7 || (accessPower == 6 && levels.HighestLevelValue == 3)) && UnityEngine.Random.Range(0, 1) == 1)
    //                        return destroyedResult();

    //                    ResultingLevels = upgradeExpandedCardFine(new(levels.Containment + 1, levels.Armory + 1, levels.Admin + 1));
    //                }
    //                break;
    //            case Scp914KnobSetting.Fine:
    //                if (accessPower == 9 && gambling == 0)
    //                    return destroyedResult();

    //                ResultingLevels = upgradeExpandedCardFine(levels);
    //                break;
    //            case Scp914KnobSetting.OneToOne:
    //                ResultingLevels = new(levels.Armory, levels.Admin, levels.Containment);
    //                break;
    //            case Scp914KnobSetting.Coarse:
    //            coarse:
    //                if (accessPower == 0)
    //                    return destroyedResult();

    //                int[] Access = [levels.Containment, levels.Armory, levels.Admin];
    //                int highest = 0;

    //                int loss = (accessPower + 2) / 3;
    //                while (loss > 0)
    //                {
    //                    for (int i = 0; i < 3; i++)
    //                        if (Access[i] > Access[highest])
    //                            highest = i;

    //                    Access[highest]--;
    //                    loss--;
    //                }

    //                ResultingLevels = new(Access[0], Access[1], Access[2]);
    //                break;
    //            case Scp914KnobSetting.Rough:
    //                if (gambling == 0)
    //                {
    //                    goto coarse;
    //                }
    //                else
    //                {
    //                    if (accessPower < gambling + 1)
    //                        destroyedResult();

    //                    accessPower += accessPower / 2;
    //                    goto coarse;
    //                }
    //        }

    //        int pIndex = -1;
    //        if (ResultingLevels.Containment + ResultingLevels.Armory + ResultingLevels.Admin == 9 && UnityEngine.Random.Range(0f, 1f) > 0.02f)
    //        {
    //            pIndex = 0; // O5 card more than 98% of the time
    //        }

    //        InventorySystem.Items.Keycards.KeycardItem result = CardsExpandedPlugin.ExpCards.GetExpandedKeycard(ResultingLevels, owner, pIndex);

    //    }

    //    private KeycardLevels upgradeExpandedCardFine(KeycardLevels levels) // What a mess...
    //    {
    //        int[] Access = [levels.Containment, levels.Armory, levels.Admin];

    //        int n3 = Access.Count(x => x == 3);
    //        int n2 = Access.Count(x => x == 2);
    //        int n1 = Access.Count(x => x == 1);
    //        int n0 = Access.Count(x => x == 0);

    //        if ((n3 >= 2) || (n2 == 2 && n3 == 1) || (n2 == 3)) return new(3, 3, 3);

    //        if (n2 == 2)
    //        {
    //            for (int i = 0; i < 3; i++)
    //                if (Access[i] == 2) Access[i]++;
    //            return new(Access[0], Access[1], Access[2]);
    //        }

    //        if (n0 == 2)
    //        {
    //            if (n1 == 1)
    //            {
    //                for (int i = 0; i < 3; i++)
    //                    if (Access[i] == 1)
    //                    {
    //                        Access[i]++;
    //                        Access[(i - 1) % 3]++;
    //                        break;
    //                    }
    //            }
    //            else
    //                for (int i = 0; i < 3; i++)
    //                    if (Access[i] == 0) Access[i]++;

    //            return new(Access[0], Access[1], Access[2]);
    //        }

    //        for (int i = 0; i < 3; i++)
    //            if (Access[i] == 1 || Access[i] == 2) Access[i]++;

    //        if (n0 == 3) Access[0]++;

    //        return new(Access[0], Access[1], Access[2]);
    //    }
    //}
}
