using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.S3.Publisher.Helpers;

namespace TME.CarConfigurator.S3.Publisher.Extensions
{
    public static class AssetExtensions
    {
        public static IEnumerable<Asset> DefaultAssets(this IEnumerable<Asset> assets)
        {
            return assets.Where(a => String.IsNullOrEmpty(a.AssetType.Mode) || String.IsNullOrEmpty(a.AssetType.View));
        }

        public static IEnumerable<IGrouping<ModeAndView, Asset>> GroupedByModeAndView(this IEnumerable<Asset> assets)
        {
            return assets
                .Where(a => !String.IsNullOrEmpty(a.AssetType.Mode) && !String.IsNullOrEmpty(a.AssetType.View))
                .GroupBy(a => new ModeAndView(a.AssetType));
        }
    }
}