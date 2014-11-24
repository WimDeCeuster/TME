using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IAssetSetMapper
    {
        IEnumerable<VisibleInModeAndView> GetVisibility(Administration.Assets.AssetSet assetSet, bool canHaveAssets);
        IEnumerable<VisibleInModeAndView> GetVisibility(Administration.Assets.LinkedAssets linkedAssets, bool canHaveAssets);
        IEnumerable<VisibleInModeAndView> GetVisibility(IEnumerable<Administration.Assets.AssetSetAsset> assetSetAssets, bool canHaveAssets);
    }
}
