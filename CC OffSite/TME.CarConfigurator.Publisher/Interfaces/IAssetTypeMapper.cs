using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Administration.Assets;
using AssetType = TME.CarConfigurator.Repository.Objects.Assets.AssetType;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IAssetTypeMapper
    {
        AssetType MapGenerationAssetType(Administration.Assets.AssetType assetType);

        AssetType MapObjectAssetType(AssetSetAsset assetSetAsset, ModelGeneration modelGeneration);
        AssetType MapCarAssetType(AssetSetAsset asset, ModelGeneration modelGeneration);
    }
}
