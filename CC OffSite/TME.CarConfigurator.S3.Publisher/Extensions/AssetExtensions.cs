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
            return assets.Where(a => String.IsNullOrEmpty(a.AssetType.View)).Ordered();
        }

        public static IOrderedEnumerable<Asset> Ordered(this IEnumerable<Asset> assets)
        {
            return assets.OrderBy(asset => asset.Name).ThenBy(asset => asset.AssetType.Name).ThenBy(asset => asset.ShortID);
        } 

        public static Dictionary<Guid, IList<Asset>> DefaultAssets(this IDictionary<Guid, IList<Asset>> carItemAssets)
        {
            return carItemAssets.ToDictionary(
                entry => entry.Key,
                entry => (IList<Asset>)entry.Value.DefaultAssets().ToList());
        }

        public static IDictionary<ModeAndView, List<Asset>> ByModeAndView(this IEnumerable<Asset> assets)
        {
            return assets
                .Where(a => !String.IsNullOrEmpty(a.AssetType.View))
                .GroupBy(a => new ModeAndView(a.AssetType))
                .ToDictionary(group => group.Key, group => group.ToList());
        }

        public static IDictionary<ModeAndView, Dictionary<Guid, IList<Asset>>> ByModeAndView(this IDictionary<Guid, IList<Asset>> carItemAssets)
        {
            var splitCarItemAssets = carItemAssets.ToDictionary(
                entry => entry.Key,
                entry => entry.Value.ByModeAndView());

            var modeAndViews = splitCarItemAssets.SelectMany(entry => entry.Value.Keys).Distinct();

            return modeAndViews.ToDictionary(
                modeAndView => modeAndView,
                modeAndView => splitCarItemAssets.Where(entry => entry.Value.ContainsKey(modeAndView))
                                                 .ToDictionary(
                                                    entry => entry.Key,
                                                    entry => (IList<Asset>)entry.Value[modeAndView].Ordered().ToList()));
        }
    }
}