using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Administration.Assets;
using TME.CarConfigurator.Administration.Components;
using TME.CarConfigurator.Administration.Interfaces;

namespace TME.CarConfigurator.Publisher.Extensions
{
    public static class ComponentExtensions
    {
        public static IEnumerable<AssetSetAsset> GetFilteredAssets(this ModelGenerationOptionComponents components, Car car)
        {
            return components.Where(comp => ((IHasAssetSet)comp).AssetSet.NumberOfAssets != 0).SelectMany(component => ((IHasAssetSet)component).AssetSet.Assets.Filter(car)).Distinct();
        }
    }
}