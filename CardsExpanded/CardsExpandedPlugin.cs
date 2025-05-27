using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardsExpanded.Commands;
using CardsExpanded.Features;
using LabApi;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using Scp914.Processors;

namespace CardsExpanded
{
    public class CardsExpandedPlugin : Plugin<Config>
    {
        public override string Name => "Cards Expanded";

        public override string Description => "Adds in 54 new cards to the game";

        public override string Author => "SpiderBuh";

        public override Version Version => new(0, 1, 0, 0);

        public override Version RequiredApiVersion => new(LabApi.Features.LabApiProperties.CompiledVersion);

        public static ExpandedCards ExpCards;
        private UpgradeEvents ug;
        public static ExpandedCardProcessor CardProcessor;

        public static List<ItemType> KeycardItemTypes = Enum.GetValues(typeof(ItemType)).ToArray<ItemType>().Where(x => x.ToString().Contains("Keycard")).ToList();

        public override void Enable()
        {
            try
            {
                ExpCards = new(this.GetConfigPath("AllCards.json").Replace(".yml",""));
            } catch (FileNotFoundException e)
            {
                Logger.Error($"Could not find/read the AllCards.json file.\n{e.ToString()}");
                return;
            }

            ug = new();
            CustomHandlersManager.RegisterEventsHandler(ug);
            CardProcessor = new();
            foreach (var itemType in KeycardItemTypes)
                LabApi.Features.Wrappers.Scp914.SetItemProcessor(itemType, CardProcessor);
        }

        public override void Disable()
        {
            // Uhhhh server restart or something idk
        }

    }
}
