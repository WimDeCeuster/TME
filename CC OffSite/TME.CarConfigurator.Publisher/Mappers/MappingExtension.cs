using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Administration.Assets;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public static class MappingExtension
    {
        public static String DefaultIfEmpty(this String str, String defaultStr)
        {
            return String.IsNullOrWhiteSpace(str) ? defaultStr : str;
        }

        public static IEnumerable<AssetSetAsset> GetGenerationAssets(this IEnumerable<AssetSetAsset> assets)
        {
            return assets.Where(asset => !asset.IsDeviation() && asset.AlwaysInclude);
        }
    }
}
