using System.Collections.Generic;
using System.Linq;

namespace TME.CarConfigurator.Publisher.Factory.Assets
{
    internal static class VisibleInModeAndView
    {
        public static List<Repository.Objects.Assets.VisibleInModeAndView> CreateList(Administration.Assets.AssetSet assetSet)
        {

            var result = (
                assetSet.Assets.Select(asset => 
                    new Repository.Objects.Assets.VisibleInModeAndView
                        {
                            Mode = asset.AssetType.Details.Mode,
                            View = asset.AssetType.Details.View
                        }
               )).Distinct().ToList();

            return result;
        }
    }
}
