using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class AssetSetMapper : IAssetSetMapper
    {
        public IEnumerable<VisibleInModeAndView> GetVisibility(Administration.Assets.AssetSet assetSet)
        {
            return assetSet.Assets.Select(asset => Tuple.Create(asset.AssetType.Details.Mode, asset.AssetType.Details.View))
                                  .Distinct()
                                  .Select(info => new VisibleInModeAndView { Mode = info.Item1, View = info.Item2 })
                                  .Where(info => !String.IsNullOrWhiteSpace(info.View));
        }

        public IEnumerable<VisibleInModeAndView> GetVisibility(Administration.Assets.LinkedAssets linkedAssets)
        {
            return linkedAssets.Select(asset => Tuple.Create(asset.AssetType.Details.Mode, asset.AssetType.Details.View))
                               .Distinct()
                               .Select(info => new VisibleInModeAndView { Mode = info.Item1, View = info.Item2 })
                               .Where(info => !String.IsNullOrWhiteSpace(info.View));
        }
    }
}
