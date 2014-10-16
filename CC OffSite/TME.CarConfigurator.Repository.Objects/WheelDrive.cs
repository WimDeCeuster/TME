using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects
{
    public class WheelDrive : BaseObject
    {
        public Boolean KeyFeature { get; set; }
        public Boolean Brochure { get; set; }

        public List<VisibleInModeAndView> VisibleIn { get; set; }
    }
}
