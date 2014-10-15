using System.Runtime.Serialization;
using TME.CarConfigurator.Repository.Objects.Assets.Enums;

namespace TME.CarConfigurator.Repository.Objects.Assets
{
    public class AssetType
    {

        public string Code { get; set; }

        public string Name { get; set; }

        public string Mode { get; set; }

        public string View { get; set; }

        public string Side { get; set; }

        public string Type { get; set; }

        public string ExteriorColourCode { get; set; }

        public string UpholsteryCode { get; set; }

        public string EquipmentCode { get; set; }
   }
}