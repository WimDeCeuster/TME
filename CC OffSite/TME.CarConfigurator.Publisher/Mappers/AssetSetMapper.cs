using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Administration.Assets;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class AssetSetMapper : IAssetSetMapper
    {
        public IEnumerable<VisibleInModeAndView> GetVisibility(Administration.Assets.AssetSet assetSet)
        {
            return GetVisibility(assetSet.Assets.Select(asset => asset.AssetType));
        }

        public IEnumerable<VisibleInModeAndView> GetVisibility(Administration.Assets.LinkedAssets linkedAssets)
        {
            return GetVisibility(linkedAssets.Select(asset => asset.AssetType));
        }

        public IEnumerable<VisibleInModeAndView> GetVisibility(IEnumerable<AssetSetAsset> assetSetAssets)
        {
            return GetVisibility(assetSetAssets.Select(asset => asset.AssetType));
        }

        private static IEnumerable<VisibleInModeAndView> GetVisibility(IEnumerable<Administration.Assets.AssetType> assetTypes)
        {
            return assetTypes.Where(assetType => !String.IsNullOrWhiteSpace(assetType.Details.View))
                             .Select(assetType => Tuple.Create(assetType.Details.Mode, assetType.Details.View))
                             .Distinct()
                             .Select(info => new VisibleInModeAndView { Mode = info.Item1, View = info.Item2 });
        }
    }
}
