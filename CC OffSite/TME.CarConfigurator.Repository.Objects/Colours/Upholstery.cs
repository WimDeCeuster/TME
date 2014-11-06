using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects.Colours
{
    public class Upholstery : BaseObject
    {
        public string InteriorColourCode { get; set; }
        public string TrimCode { get; set; }
        public UpholsteryType Type { get; set; }

        public List<VisibleInModeAndView> VisibleIn { get; set; }
    }
}
