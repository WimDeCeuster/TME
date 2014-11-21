using System.Collections.Generic;
using TME.CarConfigurator.Administration.Assets;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IAssetSetMapper
    {
        IEnumerable<VisibleInModeAndView> GetVisibility(Administration.Assets.AssetSet assetSet);
        IEnumerable<VisibleInModeAndView> GetVisibility(Administration.Assets.LinkedAssets linkedAssets);
        IEnumerable<VisibleInModeAndView> GetVisibility(IEnumerable<AssetSetAsset> assetSetAssets);
    }
}
