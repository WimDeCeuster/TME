using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.LegacyAdapter.Assets;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter.Extensions
{
    internal static class AssetExtensions
    {
        public static IReadOnlyList<IVisibleInModeAndView> GetVisibleInModeAndViews(this Legacy.Assets assets)
        {
            var groups = assets.Cast<Legacy.Asset>()
                    .Where(x => x.AssetType.Details.View == "EXT" || x.AssetType.Details.View == "INT")
                    .GroupBy(x => new { x.AssetType.Details.Mode, x.AssetType.Details.View })
                    .Select(group => new VisibleInModeAndView()
                    {
                        Mode = group.Key.Mode,
                        View = group.Key.View,
                        Assets = group.Select(legacyAsset => new Asset(legacyAsset))
                                      .OrderBy(x => x.Name).ThenBy(x => x.AssetType.Name)
                                      .ToList()
                    })
                    .ToList();
            return groups;
        }

        public static IReadOnlyList<IAsset> GetPlainAssets(this Legacy.Assets assets)
        {
             return assets.Cast<Legacy.Asset>()
                    .Where(x => x.AssetType.Details.View != "EXT" && x.AssetType.Details.View != "INT")
                    .Select(x => new Asset(x))
                    .OrderBy(x => x.Name).ThenBy(x => x.AssetType.Name)
                    .ToList(); 
        }
     }
 }

