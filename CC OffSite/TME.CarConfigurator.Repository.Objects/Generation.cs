using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects
{
    
    public class Generation : BaseObject
    {
        public String SSN { get; set; }

        public CarConfiguratorVersion CarConfiguratorVersion { get; set; }

        public List<Link> Links { get; set; }

        public List<Asset> Assets { get; set; }
    }
}