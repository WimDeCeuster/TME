using System;
using System.Collections.Generic;
using System.Linq;

using TME.CarConfigurator.Interfaces.Assets;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter.Extensions
{
    internal static class AssetExtensions
    {
        public static IEnumerable<IVisibleInModeAndView> GetVisibleInModeAndViews(this Legacy.Assets assets)
        {
            var groups = assets.Cast<Legacy.Asset>()
                    .Where(x => x.AssetType.Mode.Length == 0)
                    .GroupBy(x => new { x.AssetType.Details.Mode, x.AssetType.Details.View })
                    .Select(group => new VisibleInModeAndView()
                    {
                        Mode = group.Key.Mode,
                        View = group.Key.View,
                        Assets = group.Select(legacyAsset => new Asset(legacyAsset))

                    })
                    .ToList();
            return groups;
        }

        public static IEnumerable<IAsset> GetPlainAssets(this Legacy.Assets assets)
        {
             return assets.Cast<Legacy.Asset>()
                    .Where(x=>x.AssetType.Mode.Length == 0)
                    .Select(x => new Asset(x)); 
        }
     }
 }

