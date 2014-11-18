using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.Repository.Objects
{
    public class CarPart
    {
        public Guid ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }


        public IList<VisibleInModeAndView> VisibleIn { get; set; }
    }
}
