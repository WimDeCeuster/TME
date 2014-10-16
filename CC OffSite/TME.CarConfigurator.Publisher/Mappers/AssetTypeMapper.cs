using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Administration.Assets;
using TME.CarConfigurator.Publisher.Interfaces;
using AssetType = TME.CarConfigurator.Repository.Objects.Assets.AssetType;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class AssetTypeMapper : IAssetTypeMapper
    {
        public AssetType MapGenerationAssetType(Administration.Assets.AssetType assetType)
        {
            return new AssetType
            {
                Code = assetType.Code,
                EquipmentCode = String.Empty,
                ExteriorColourCode = String.Empty,
                Mode = assetType.Details.Mode,
                Name = assetType.Name,
                Side = assetType.Details.Side,
                Type = assetType.Details.Type,
                UpholsteryCode = String.Empty,
                View = assetType.Details.View
            };
        }

        public AssetType MapObjectAssetType(AssetSetAsset assetSetAsset,ModelGeneration modelGeneration)
        {
            var equipmentCode = String.Empty;
            if (assetSetAsset.EquipmentItem.ID != Guid.Empty)
                equipmentCode = modelGeneration.Equipment[assetSetAsset.EquipmentItem.ID].BaseCode;
            
            return new AssetType()
            {
                Code = assetSetAsset.AssetType.Code,
                EquipmentCode =  equipmentCode,
                ExteriorColourCode = assetSetAsset.ExteriorColour.Code,
                Mode = assetSetAsset.AssetType.Details.Mode,
                Name = assetSetAsset.AssetType.Name,
                Side = assetSetAsset.AssetType.Details.Side,
                Type = assetSetAsset.AssetType.Details.Type,
                UpholsteryCode = assetSetAsset.Upholstery.Code,
                View = assetSetAsset.AssetType.Details.View
            };
        }
    }
}
