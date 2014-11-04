using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;

using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Assets;

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

        public async Task PublishAssetsAsync(IContext context)
        {
            await Task.WhenAll(context.ContextData.Keys.Select(languageCode => PublishAssets(context.Brand, context.Country, context.ContextData[languageCode].Publication.ID, context.ContextData[languageCode].Assets)));
        }

        private async Task PublishAssets(String brand, String country, Guid publicationID, IDictionary<Guid, IList<Asset>> assetsPerObjectID)
        {
            await Task.WhenAll(assetsPerObjectID.Keys.Select(objectID => PublishAssets(assetsPerObjectID[objectID],
                async assets => await PublishAssetsByModeAndView(brand, country, publicationID, objectID, assets),
                async assets => await PublishDefaultAssets(brand, country, publicationID, objectID, assets))));
        }

        private static async Task PublishAssets(IEnumerable<Asset> assets, Func<IEnumerable<Asset>, Task> getModeAndViewTasks, Func<IEnumerable<Asset>, Task> getDefaultTask)
        {
            var orderedAssets = assets.OrderBy(asset => asset.Name).ThenBy(asset => asset.AssetType.Name).ToList();

            var modeAndViewTasks = getModeAndViewTasks(orderedAssets);
            var defaultTask = getDefaultTask(orderedAssets);

            await modeAndViewTasks;
            await defaultTask;
        }

        private async Task PublishAssetsByModeAndView(String brand, String country, Guid publicationID, Guid objectID, IEnumerable<Asset> assets)
        {
            var assetsByModeAndView = GetAssetsGroupedByModeAndView(assets);

            var tasks = assetsByModeAndView.Select(grouping => _assetService.PutAssetsByModeAndView(brand, country, publicationID, objectID, grouping.Key.Mode, grouping.Key.View, grouping));

            await Task.WhenAll(tasks);
        }

        private static IEnumerable<IGrouping<AssetTypeKey, Asset>> GetAssetsGroupedByModeAndView(IEnumerable<Asset> assets)
        {
            return assets.Where(a => !String.IsNullOrEmpty(a.AssetType.Mode) || !String.IsNullOrEmpty(a.AssetType.View))
                .GroupBy(a => new AssetTypeKey(a.AssetType));
        }

        private async Task PublishDefaultAssets(String brand, String country, Guid publicationID, Guid objectID, IEnumerable<Asset> assets)
        {
            var defaultAssets = GetDefaultAssets(assets);

            await _assetService.PutDefaultAssets(brand, country, publicationID, objectID, defaultAssets);
        }

        private static IEnumerable<Asset> GetDefaultAssets(IEnumerable<Asset> assets)
        {
            return assets.Where(a => String.IsNullOrEmpty(a.AssetType.Mode) || String.IsNullOrEmpty(a.AssetType.View));
        }

        public async Task PublishCarAssetsAsync(IContext context)
        {
            var tasks = context.ContextData.Keys.Select(languageCode => PublishAssets(context.Brand, context.Country, context.ContextData[languageCode].Publication.ID, context.ContextData[languageCode].CarAssets));
            await Task.WhenAll(tasks);
        }

        private async Task PublishAssets(string brand, string country, Guid publicationID, IDictionary<Guid, IDictionary<Guid, IList<Asset>>> carAssets)
        {
            await Task.WhenAll(carAssets.Keys.Select(carId => PublishAssets(brand, country, publicationID, carId, carAssets[carId])));
        }
        private async Task PublishAssets(string brand, string country, Guid publicationID, Guid carId, IDictionary<Guid, IList<Asset>> assetsPerObjectID)
        {
            await Task.WhenAll(assetsPerObjectID.Keys.Select(objectID => PublishAssets(assetsPerObjectID[objectID],
                async assets => await PublishAssetsByModeAndView(brand, country, publicationID, carId, objectID, assets),
                async assets => await PublishDefaultAssets(brand, country, publicationID, carId, objectID, assets))));
        }

        private async Task PublishDefaultAssets(string brand, string country, Guid publicationID, Guid carId, Guid objectID, IEnumerable<Asset> assets)
        {
            var defaultAssets = GetDefaultAssets(assets);

            await _assetService.PutDefaultAssets(brand, country, publicationID, carId, objectID, defaultAssets);
        }

        private async Task PublishAssetsByModeAndView(string brand, string country, Guid publicationID, Guid carId, Guid objectID, IEnumerable<Asset> assets)
        {
            var assetsByModeAndView = GetAssetsGroupedByModeAndView(assets);

            var tasks = assetsByModeAndView.Select(grouping => _assetService.PutAssetsByModeAndView(brand, country, publicationID, carId, objectID, grouping.Key.Mode, grouping.Key.View, grouping));

            await Task.WhenAll(tasks);
        }

        private class AssetTypeKey : IEquatable<AssetTypeKey>
        {
            public string Mode { get; private set; }
            public string View { get; private set; }

            public AssetTypeKey(AssetType assetType)
            {
                Mode = assetType.Mode;
                View = assetType.View;
            }

            public bool Equals(AssetTypeKey other)
            {
                return Mode.Equals(other.Mode) && View.Equals(other.View);
            }

            public override int GetHashCode()
            {
                return string.Format("{0}{1}", Mode, View).GetHashCode();
            }
        }
    }

}