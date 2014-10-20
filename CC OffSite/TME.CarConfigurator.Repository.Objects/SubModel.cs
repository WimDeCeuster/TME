using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects
{
    public class SubModel : BaseObject
    {
        public Generation Generation { get; set; }

        public List<VisibleInModeAndView> VisibleIn { get; set; }
        public List<Asset> Assets { set; get; }
    }
}