using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects.Packs
{
    public class CarPack : Pack
    {

        public bool Standard { get; set; }
        public bool Optional { get; set; }

        public Price Price { get; set; }

        List<ExteriorColourInfo> AvailableForExteriorColours { get; set; }
        List<UpholsteryInfo> AvailableForUpholsteries { get; set; }
    }
}
