using System;
using TME.CarConfigurator.Interfaces.Assets;

namespace TME.CarConfigurator.Assets
{
    public class AssetType : IAssetType
    {
        private readonly Repository.Objects.Assets.AssetType _repositoryAssetType;

        public AssetType(Repository.Objects.Assets.AssetType repositoryAssetType)
        {
            if (repositoryAssetType == null) throw new ArgumentNullException("repositoryAssetType");
            _repositoryAssetType = repositoryAssetType;
        }

        public string Code { get{ return _repositoryAssetType.Code; } }
        public string Name { get{ return _repositoryAssetType.Name; } }
        public string Mode { get{ return _repositoryAssetType.Mode; } }
        public string View { get{ return _repositoryAssetType.View; } }
        public string Side { get{ return _repositoryAssetType.Side; } }
        public string Type { get{ return _repositoryAssetType.Type; } }
        public string ExteriorColourCode { get{ return _repositoryAssetType.ExteriorColourCode; } }
        public string UpholsteryCode { get{ return _repositoryAssetType.UpholsteryCode; } }
        public string EquipmentCode { get{ return _repositoryAssetType.EquipmentCode; } }
    }
}