using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;

using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.CarConfigurator.S3.Publisher
{
    public class AssetPublisher : IAssetPublisher
    {
        private readonly IAssetService _assetService;
        private readonly ITimeFramePublishHelper _timeFramePublishHelper;

        public AssetPublisher(IAssetService assetService, ITimeFramePublishHelper timeFramePublishHelper)
        {
            if (assetService == null) throw new ArgumentNullException("assetService");
            if (timeFramePublishHelper == null) throw new ArgumentNullException("timeFramePublishHelper");

            _assetService = assetService;
            _timeFramePublishHelper = timeFramePublishHelper;
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
            return assets.Where(a => String.IsNullOrEmpty(a.AssetType.Mode) && String.IsNullOrEmpty(a.AssetType.View));
        }

        public async Task PublishCarAssetsAsync(IContext context)
        {
            var tasks = context.ContextData.Keys.Select(languageCode => PublishCarAssets(context.Brand, context.Country, context.ContextData[languageCode].Publication.ID, context.ContextData[languageCode].CarAssets));
            await Task.WhenAll(tasks);
        }

        public async Task PublishSubModelAssetsAsync(IContext context)
        {
            await _timeFramePublishHelper.PublishObjectsPerSubModel(context, timeFrame => timeFrame.SubModels, timeFrame => timeFrame.SubModelAssets, SubModelPublish);
        }

        private async Task SubModelPublish(string brand, string country, Guid publicationID, Guid timeFrameID, Guid subModelID, List<Grade> grades, IReadOnlyDictionary<Guid, IDictionary<Guid, IList<Asset>>> subModelAssetsPerObject)
        {
            var assetsPerObject =
                subModelAssetsPerObject.Where(entry => entry.Key == subModelID)
                    .SelectMany(entry => entry.Value)
                    .ToList();

            var tasks = new List<IEnumerable<Task>>();
            var task = new List<Task>();
            
            foreach (var entry in assetsPerObject)
            {
                var assetsByModeAndView = GetAssetsGroupedByModeAndView(entry.Value);
                tasks.Add(assetsByModeAndView.Select(grouping => _assetService.PutSubModelAssetsByModeAndView(brand, country, publicationID, timeFrameID, subModelID, entry.Key, grouping.Key.Mode, grouping.Key.View, grouping)));
                var assetsByDefault = GetDefaultAssets(entry.Value);
                task.Add(_assetService.PutSubModelDefaultAssets(brand, country, publicationID, timeFrameID, subModelID, entry.Key, assetsByDefault));
                tasks.Add(task);
            }
            
            await Task.WhenAll(tasks.SelectMany(t => t));
        }

        private async Task PublishCarAssets(string brand, string country, Guid publicationID, IDictionary<Guid, IDictionary<Guid, IList<Asset>>> carAssets)
        {
            await Task.WhenAll(carAssets.Keys.Select(carId => PublishCarAssets(brand, country, publicationID, carId, carAssets[carId])));
        }

        private async Task PublishCarAssets(string brand, string country, Guid publicationID, Guid objectID, IDictionary<Guid, IList<Asset>> assetsPerObjectID)
        {
            await Task.WhenAll(assetsPerObjectID.Keys.Select(subObjectID => PublishAssets(assetsPerObjectID[subObjectID],
                async assets => await PublishCarAssetsByModeAndView(brand, country, publicationID, objectID, subObjectID, assets),
                async assets => await PublishCarDefaultAssets(brand, country, publicationID, objectID, subObjectID, assets))));
        }

        private async Task PublishCarDefaultAssets(string brand, string country, Guid publicationID, Guid carId, Guid objectID, IEnumerable<Asset> assets)
        {
            var defaultAssets = GetDefaultAssets(assets);

            await _assetService.PutCarDefaultAssets(brand, country, publicationID, carId, objectID, defaultAssets);
        }

        private async Task PublishCarAssetsByModeAndView(string brand, string country, Guid publicationID, Guid carId, Guid objectID, IEnumerable<Asset> assets)
        {
            var assetsByModeAndView = GetAssetsGroupedByModeAndView(assets);

            var tasks = assetsByModeAndView.Select(grouping => _assetService.PutCarAssetsByModeAndView(brand, country, publicationID, carId, objectID, grouping.Key.Mode, grouping.Key.View, grouping));

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