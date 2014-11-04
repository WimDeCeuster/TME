using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.Repository.Objects.Colours
{
    public class ColourCombination
    {
        public Guid ID { get; set; }
        public ExteriorColour ExteriorColour { get; set; }
        public Upholstery Upholstery { get; set; }
        public int SortIndex { get; set; }

        public List<VisibleInModeAndView> VisibleIn { get; set; }
    }
}
