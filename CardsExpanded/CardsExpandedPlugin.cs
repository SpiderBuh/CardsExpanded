using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardsExpanded.Commands;
using CardsExpanded.Features;
using LabApi;
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

        internal ExpandedCardProcessor cardProcessor;

        public static List<ItemType> KeycardItemTypes = Enum.GetValues(typeof(ItemType)).ToArray<ItemType>().Where(x => x.ToString().Contains("Keycard")).ToList();

        public override void Enable()
        {
            cardProcessor = new();
            foreach (var itemType in KeycardItemTypes)
                LabApi.Features.Wrappers.Scp914.SetItemProcessor(itemType, cardProcessor);
        }

        public override void Disable()
        {
            // Uhhhh server restart or something idk
        }

    }
}
