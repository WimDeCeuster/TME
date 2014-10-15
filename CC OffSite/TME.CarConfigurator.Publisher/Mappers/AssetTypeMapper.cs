using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class AssetTypeMapper : IAssetTypeMapper
    {
        readonly IAssetScopeMapper _assetScopeMapper;

        public AssetTypeMapper(IAssetScopeMapper assetScopeMapper)
        {
            if (assetScopeMapper == null) throw new ArgumentNullException("assetScopeMapper");

            _assetScopeMapper = assetScopeMapper;
        }

        public AssetType MapGenerationAssetType(Administration.Assets.AssetType assetType)
        {
            return new AssetType
            {
                Code = assetType.Code,
                EquipmentCode = String.Empty,
                ExteriorColourCode = String.Empty,
                Mode = assetType.Details.Mode,
                Name = assetType.Name,
                Scope = _assetScopeMapper.MapAssetScope(assetType.Group.Scope),
                Side = assetType.Details.Side,
                Type = assetType.Details.Type,
                UpholsteryCode = String.Empty,
                View = assetType.Details.View
            };
        }
    }
}
