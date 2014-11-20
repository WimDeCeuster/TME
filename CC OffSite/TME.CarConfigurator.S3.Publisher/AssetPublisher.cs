using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.S3.Publisher.Extensions;

namespace TME.CarConfigurator.S3.Publisher
{
    public class AssetPublisher : IAssetPublisher
    {
        private delegate Task PublishAssetsByModeAndViewDelegate(string brand, string country, Guid publicationID, Guid carID, Guid objectID, string mode, string view, IEnumerable<Asset> assets);
        private delegate Task PublishDefaultAssetsDelegate(string brand, string country, Guid publicationID, Guid carID, Guid objectID, IEnumerable<Asset> assets);
        private delegate Task PublishCarItemsByModeAndView(string brand, string country, Guid publicationID, Guid carID, string mode, string view, Dictionary<Guid, IList<Asset>> carItemAssets);
        private delegate Task PublishDefaultCarItems(string brand, string country, Guid publicationID, Guid carID, Dictionary<Guid, IList<Asset>> carItemAssets);

        private readonly IAssetService _assetService;

        public AssetPublisher(IAssetService assetService)
        {
            if (assetService == null) throw new ArgumentNullException("assetService");

            _assetService = assetService;
        }

        public async Task PublishAsync(IContext context)
        {
            var tasks = new List<Task>();

            foreach (var contextData in context.ContextData.Values)
            {
                tasks.Add(PublishAsync(context.Brand, context.Country, contextData.Publication.ID, contextData.Assets));
                tasks.Add(PublishAsync(context.Brand, context.Country, contextData.Publication.ID, contextData.CarAssets, _assetService.PutCarAssetsByModeAndView, _assetService.PutDefaultCarAssets));
                tasks.Add(PublishAsync(context.Brand, context.Country, contextData.Publication.ID, contextData.CarPartAssets, _assetService.PutCarPartsAssetsByModeAndView, null));
                tasks.Add(PublishAsync(context.Brand, context.Country, contextData.Publication.ID, contextData.CarEquipmentAssets, _assetService.PutCarEquipmentAssetsByModeAndView, _assetService.PutDefaultCarEquipmentAssets));
                tasks.Add(PublishAsync(context.Brand, context.Country, contextData.Publication.ID, contextData.SubModelAssets, _assetService.PutSubModelAssetsByModeAndView, _assetService.PutDefaultSubModelAssets));
            }

            await Task.WhenAll(tasks);
        }

        private async Task PublishAsync(string brand, string country, Guid publicationID, IDictionary<Guid, IDictionary<Guid, IList<Asset>>> assetsPerCar, PublishCarItemsByModeAndView putCarEquipmentAssetsByModeAndView, PublishDefaultCarItems putDefaultCarEquipmentAssets)
        {
            await Task.WhenAll(assetsPerCar.Keys.Select(async carID =>
            {
                await PublishCarItemAssetsByModeAndViewAsync(brand, country, publicationID, carID, assetsPerCar[carID], putCarEquipmentAssetsByModeAndView);
                if (putDefaultCarEquipmentAssets != null)
                    await PublishDefaultCarItemAssets(brand, country, publicationID, carID, assetsPerCar[carID], putDefaultCarEquipmentAssets);
            }));
        }

        private async Task PublishAsync(string brand, string country, Guid publicationID, IDictionary<Guid, IList<Asset>> assetsPerObjectID)
        {
            await Task.WhenAll(assetsPerObjectID.Keys.Select(objectID => PublishAsync(assetsPerObjectID[objectID],
                async assets => await PublishAssetsByModeAndViewAsync(brand, country, publicationID, objectID, assets),
                async assets => await PublishDefaultAssetsAsync(brand, country, publicationID, objectID, assets))));
        }

        private static async Task PublishAsync(string brand, string country, Guid publicationID, IDictionary<Guid, IDictionary<Guid, IList<Asset>>> objectAssets, PublishAssetsByModeAndViewDelegate publishAssetsByModeAndViewDelegate, PublishDefaultAssetsDelegate publishDefaultAssetsDelegate)
        {
            await Task.WhenAll(objectAssets.Keys.Select(objectID => PublishAsync(brand, country, publicationID, objectID, objectAssets[objectID], publishAssetsByModeAndViewDelegate, publishDefaultAssetsDelegate)));
        }

        private static async Task PublishAsync(string brand, string country, Guid publicationID, Guid objectID, IDictionary<Guid, IList<Asset>> assetsPerObjectID, PublishAssetsByModeAndViewDelegate publishByModeAndViewDelegate, PublishDefaultAssetsDelegate publishDefaultAssetsDelegate)
        {
            await Task.WhenAll(assetsPerObjectID.Keys.Select(subObjectID => PublishAsync(assetsPerObjectID[subObjectID],
                async assets => await PublishAssetsByModeAndViewAsync(brand, country, publicationID, objectID, subObjectID, assets, publishByModeAndViewDelegate),
                async assets => await PublishDefaultAssetsAsync(brand, country, publicationID, objectID, subObjectID, assets, publishDefaultAssetsDelegate))));
        }

        private static async Task PublishAsync(IEnumerable<Asset> assets, Func<IEnumerable<Asset>, Task> getModeAndViewTasks, Func<IEnumerable<Asset>, Task> getDefaultTask)
        {
            var orderedAssets = assets.Ordered().ToList();

            var modeAndViewTasks = getModeAndViewTasks(orderedAssets);
            var defaultTask = getDefaultTask(orderedAssets);

            await modeAndViewTasks;
            await defaultTask;
        }

        private async Task PublishDefaultAssetsAsync(string brand, string country, Guid publicationID, Guid objectID, IEnumerable<Asset> assets)
        {
            var defaultAssets = assets.DefaultAssets();

            await _assetService.PutDefaultAssets(brand, country, publicationID, objectID, defaultAssets);
        }

        private static async Task PublishDefaultCarItemAssets(string brand, string country, Guid publicationID, Guid carID, IDictionary<Guid, IList<Asset>> carItemAssets, PublishDefaultCarItems publish)
        {
            var defaultAssets = carItemAssets.DefaultAssets();

            await publish(brand, country, publicationID, carID, defaultAssets);
        }

        private static async Task PublishDefaultAssetsAsync(string brand, string country, Guid publicationID, Guid objectID, Guid subObjectID, IEnumerable<Asset> assets, PublishDefaultAssetsDelegate publish)
        {
            var defaultAssets = assets.DefaultAssets();

            await publish(brand, country, publicationID, objectID, subObjectID, defaultAssets);
        }

        private async Task PublishCarItemAssetsByModeAndViewAsync(string brand, string country, Guid publicationID, Guid carID, IDictionary<Guid, IList<Asset>> carItemAssets, PublishCarItemsByModeAndView publish)
        {
            var assetsByModeAndView = carItemAssets.ByModeAndView();

            var tasks = assetsByModeAndView.Select(grouping => publish(brand, country, publicationID, carID, grouping.Key.Mode, grouping.Key.View, grouping.Value));

            await Task.WhenAll(tasks);
        }

        private async Task PublishAssetsByModeAndViewAsync(string brand, string country, Guid publicationID, Guid objectID, IEnumerable<Asset> assets)
        {
            var assetsByModeAndView = assets.ByModeAndView();

            var tasks = assetsByModeAndView.Select(grouping => _assetService.PutAssetsByModeAndView(brand, country, publicationID, objectID, grouping.Key.Mode, grouping.Key.View, grouping.Value));

            await Task.WhenAll(tasks);
        }

        private static async Task PublishAssetsByModeAndViewAsync(string brand, string country, Guid publicationID, Guid objectID, Guid subObjectID, IEnumerable<Asset> assets, PublishAssetsByModeAndViewDelegate publish)
        {
            var assetsByModeAndView = assets.ByModeAndView();

            var tasks = assetsByModeAndView.Select(grouping => publish(brand, country, publicationID, objectID, subObjectID, grouping.Key.Mode, grouping.Key.View, grouping.Value));

            await Task.WhenAll(tasks);
        }
    }
}