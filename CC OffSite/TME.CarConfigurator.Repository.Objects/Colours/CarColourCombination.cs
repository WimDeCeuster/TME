using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.Repository.Objects.Colours
{
    public class CarColourCombination
    {
        public Guid ID { get; set; }
        public CarExteriorColour ExteriorColour { get; set; }
        public CarUpholstery Upholstery { get; set; }
        public int SortIndex { get; set; }

        public List<VisibleInModeAndView> VisibleIn { get; set; }
    }
}
