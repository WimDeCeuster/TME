using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Administration.Assets;
using Asset = TME.CarConfigurator.Repository.Objects.Assets.Asset;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IAssetMapper
    {
        Asset MapLinkedAsset(LinkedAsset linkedAsset);
        Asset MapAssetSetAsset(AssetSetAsset assetSetAsset,ModelGeneration modelGeneration);
        Asset MapCarAssetSetAsset(AssetSetAsset asset, ModelGeneration modelGeneration);
    }
}
