using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.S3.Shared.Result;

namespace TME.CarConfigurator.S3.Publisher
{
    public class AssetPublisher : IAssetPublisher
    {
        private readonly IAssetService _assetService;

        public AssetPublisher(IAssetService assetService)
        {
            if (assetService == null) throw new ArgumentNullException("assetService");

            _assetService = assetService;
        }

        public async Task<IEnumerable<Result>> PublishAssets(IContext context)
        {
            var result = await Task.WhenAll(context.ContextData.Keys.Select(languageCode => PublishAssets(context.Brand, context.Country, context.ContextData[languageCode].Publication.ID, context.ContextData[languageCode].Assets)).ToList());
            return result.SelectMany(xs => xs.ToList());
        }

        private async Task<IEnumerable<Result>> PublishAssets(String brand, String country, Guid publicationID, IDictionary<Guid, List<Asset>> assetsPerObjectID)
        {
            var result = await Task.WhenAll(assetsPerObjectID.Keys.Select(objectID => PublishAssets(assetsPerObjectID[objectID],
                assets => PublishAssetsByModeAndView(brand, country, publicationID, objectID, assets),
                assets => PublishDefaultAssets(brand, country, publicationID, objectID, assets)))
                .ToList());
            return result.SelectMany(xs => xs.ToList());
        }

        private static async Task<IEnumerable<Result>> PublishAssets(IEnumerable<Asset> assets, Func<IEnumerable<Asset>, Task<IEnumerable<Result>>> getModeAndViewTasks, Func<IEnumerable<Asset>, Task<Result>> getDefaultTask)
        {
            var orderedAssets = assets.OrderBy(asset => asset.Name).ThenBy(asset => asset.AssetType.Name).ToList();

            var modeAndViewTasks = getModeAndViewTasks(orderedAssets);
            var defaultTask = getDefaultTask(orderedAssets);

            var modeAndViewResults = await modeAndViewTasks;
            var defaultResult = await defaultTask;

            return modeAndViewResults.Concat(new[] { defaultResult });
        }

        private async Task<IEnumerable<Result>> PublishAssetsByModeAndView(String brand, String country, Guid publicationID, Guid objectID, IEnumerable<Asset> assets)
        {
            var assetsByModeAndView = GetAssetsGroupedByModeAndView(assets);

            var tasks = assetsByModeAndView.Select(grouping => _assetService.PutAssetsByModeAndView(brand, country, publicationID, objectID, grouping.Key.Mode, grouping.Key.View, grouping)).ToList();

            return await Task.WhenAll(tasks);
        }

        private static IEnumerable<IGrouping<AssetTypeKey, Asset>> GetAssetsGroupedByModeAndView(IEnumerable<Asset> assets)
        {
            return assets.Where(a => !String.IsNullOrEmpty(a.AssetType.Mode) || !String.IsNullOrEmpty(a.AssetType.View))
                .GroupBy(a => new AssetTypeKey(a.AssetType));
        }

        private async Task<Result> PublishDefaultAssets(String brand, String country, Guid publicationID, Guid objectID, IEnumerable<Asset> assets)
        {
            var defaultAssets = GetDefaultAssets(assets);

            return await _assetService.PutDefaultAssets(brand, country, publicationID, objectID, defaultAssets);
        }

        private static IEnumerable<Asset> GetDefaultAssets(IEnumerable<Asset> assets)
        {
            return assets.Where(a => String.IsNullOrEmpty(a.AssetType.Mode) || String.IsNullOrEmpty(a.AssetType.View));
        }

        public async Task<IEnumerable<Result>> PublishCarAssets(IContext context)
        {
            var result = await Task.WhenAll(context.ContextData.Keys.Select(languageCode => PublishAssets(context.Brand, context.Country, context.ContextData[languageCode].Publication.ID, context.ContextData[languageCode].CarAssets)).ToList());
            return result.SelectMany(xs => xs.ToList());
        }

        private async Task<IEnumerable<Result>> PublishAssets(string brand, string country, Guid publicationID, IDictionary<Guid, IDictionary<Guid, IList<Asset>>> carAssets)
        {
            var result = await Task.WhenAll(carAssets.Keys.Select(carId => PublishAssets(brand, country, publicationID, carId, carAssets[carId])).ToList());
            return result.SelectMany(xs => xs.ToList());
        }

        private async Task<IEnumerable<Result>> PublishAssets(string brand, string country, Guid publicationID, Guid carId, IDictionary<Guid, IList<Asset>> assetsPerObjectID)
        {
            var result = await Task.WhenAll(assetsPerObjectID.Keys.Select(objectID => PublishAssets(assetsPerObjectID[objectID], 
                assets => PublishAssetsByModeAndView(brand, country, publicationID, carId, objectID, assets),
                assets => PublishDefaultAssets(brand, country, publicationID, carId, objectID, assets)))
                .ToList());
            return result.SelectMany(xs => xs.ToList());
        }

        private async Task<Result> PublishDefaultAssets(string brand, string country, Guid publicationID, Guid carId, Guid objectID, IEnumerable<Asset> assets)
        {
            var defaultAssets = GetDefaultAssets(assets);

            return await _assetService.PutDefaultAssets(brand, country, publicationID, carId, objectID, defaultAssets);
        }

        private async Task<IEnumerable<Result>> PublishAssetsByModeAndView(string brand, string country, Guid publicationID, Guid carId, Guid objectID, IEnumerable<Asset> assets)
        {
            var assetsByModeAndView = GetAssetsGroupedByModeAndView(assets);

            var tasks = assetsByModeAndView.Select(grouping => _assetService.PutAssetsByModeAndView(brand, country, publicationID, carId, objectID, grouping.Key.Mode, grouping.Key.View, grouping)).ToList();

            return await Task.WhenAll(tasks);
        }

        private class AssetTypeKey
        {
            public string Mode { get; private set; }
            public string View { get; private set; }

            public AssetTypeKey(AssetType assetType)
            {
                Mode = assetType.Mode;
                View = assetType.View;
            }
        }
    }

}