using System;
using System.Collections.Generic;

namespace TME.CarConfigurator.Repository.Objects.Assets
{
    public class CarPartAssets
    {
        public Guid CarPartID { get; set; }
        public List<Asset> Asset { get; set; }
    }
}
