using System;
using System.Collections.Generic;

namespace TME.CarConfigurator.Repository.Objects.Assets
{
    public class CarEquipmentAssets
    {
        public Guid EquipmentID { get; set; }
        public List<Asset> Asset { get; set; }
    }
}
