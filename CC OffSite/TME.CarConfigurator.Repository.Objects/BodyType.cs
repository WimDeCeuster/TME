using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects
{
    public class BodyType : BaseObject
    {

        public int NumberOfDoors { get; set; }

        public int NumberOfSeats { get; set; }


        public List<VisibleInModeAndView> VisibleIn { get; set; }

        public bool VisibleInExteriorSpin { get { return VisibleIn.Any(v => v.HasMode("DAY") && v.HasView("EXT")); } }
        public bool VisibleInInteriorSpin { get { return VisibleIn.Any(v => v.HasMode("DAY") && v.HasView("INT")); } }
        public bool VisibleInXRay4X4Spin { get { return VisibleIn.Any(v => v.HasMode("XRAY-4X4") && v.HasView("EXT")); } }
        public bool VisibleInXRayHybridSpin { get { return VisibleIn.Any(v => v.HasMode("XRAY-HYBRID") && v.HasView("EXT")); } }
        public bool VisibleInXRaySafetySpin { get { return VisibleIn.Any(v => v.HasMode("XRAY-SAFETY") && v.HasView("EXT")); } }
    }
}
