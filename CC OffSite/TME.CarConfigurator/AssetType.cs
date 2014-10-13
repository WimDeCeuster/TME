using System;
using TME.CarConfigurator.Interfaces.Assets;

namespace TME.CarConfigurator
{
    public class AssetType : IAssetType
    {
        private readonly Repository.Objects.Assets.AssetType _assetType;

        public AssetType(Repository.Objects.Assets.AssetType assetType)
        {
            if (assetType == null) throw new ArgumentNullException("assetType");
            _assetType = assetType;
        }

        public string Code { get{ return _assetType.Code; } }
        public string Name { get{ return _assetType.Name; } }
        public string Mode { get{ return _assetType.Mode; } }
        public string View { get{ return _assetType.View; } }
        public string Side { get{ return _assetType.Side; } }
        public string Type { get{ return _assetType.Type; } }
        public string ExteriorColourCode { get{ return _assetType.ExteriorColourCode; } }
        public string UpholsteryCode { get{ return _assetType.UpholsteryCode; } }
        public string EquipmentCode { get{ return _assetType.EquipmentCode; } }
    }
}