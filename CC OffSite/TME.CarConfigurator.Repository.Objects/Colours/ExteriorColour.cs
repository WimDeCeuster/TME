using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects.Colours
{
    public class ExteriorColour : BaseObject
    {
        public bool Promoted { get; set; }
        public ColourTransformation Transformation { get; set; }
        public ExteriorColourType Type { get; set; }

        public List<VisibleInModeAndView> VisibleIn { get; set; }
    }
}
