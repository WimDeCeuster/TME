using TME.CarConfigurator.Interfaces.Assets;

namespace TME.CarConfigurator
{
    public class AssetType : IAssetType
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string Mode { get; private set; }
        public string View { get; private set; }
        public string Side { get; private set; }
        public string Type { get; private set; }
        public string ExteriorColourCode { get; private set; }
        public string UpholsteryCode { get; private set; }
        public string EquipmentCode { get; private set; }
    }
}