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
            _assetService = assetService;
        }

        public async Task<IEnumerable<Result>> PublishAssets(IContext context)
        {
            var tasks = new List<Task<IEnumerable<Result>>>();
            foreach (var languageCode in context.ContextData.Keys)
            {
                var publicationID = context.ContextData[languageCode].Publication.ID;
                tasks.Add(PublishAssets(context.Brand, context.Country, publicationID, context.ContextData[languageCode].Assets));
            }
            var result = await Task.WhenAll(tasks);
            return result.SelectMany(xs => xs.ToList());
        }

        private async Task<IEnumerable<Result>> PublishAssets(String brand, String country, Guid publicationID, IDictionary<Guid, List<Asset>> assetsPerObjectID)
        {
            var tasks = new List<Task<IEnumerable<Result>>>();
            foreach (var objectID in assetsPerObjectID.Keys)
            {
               tasks.Add(PublishAssets(brand, country, publicationID, objectID, assetsPerObjectID[objectID]));
            }
            var result = await Task.WhenAll(tasks);
            return result.SelectMany(xs => xs.ToList());
        }

        private async Task<IEnumerable<Result>> PublishAssets(String brand, String country, Guid publicationID, Guid objectID, IList<Asset> assets)
        {
            var tasks = await PublishAssetsByModeAndView(brand, country, publicationID, objectID, assets);
            var defaultTask = await PublishDefaultAssets(brand, country, publicationID, objectID, assets);

            return tasks.Concat(new[] { defaultTask }); 
        }

        private async Task<IEnumerable<Result>> PublishAssetsByModeAndView(String brand, String country,
            Guid publicationID, Guid objectID, IEnumerable<Asset> assets)
        {
            var assetsByModeAndView = assets.Where(
                    a => !String.IsNullOrEmpty(a.AssetType.Mode) || !String.IsNullOrEmpty(a.AssetType.View))
                .OrderBy(asset => asset.Name)
                .ThenBy(asset => asset.AssetType.Name)
                .GroupBy(a => new {a.AssetType.Mode, a.AssetType.View});

            var tasks = new List<Task<Result>>();

            foreach (var assetList in assetsByModeAndView)
            {
                var mode = assetList.Key.Mode;
                var view = assetList.Key.View;
                tasks.Add(PublishAssetsByModeAndView(brand, country, publicationID, objectID, mode, view, assetList));
            }
            return await Task.WhenAll(tasks);
        }

        private async Task<Result> PublishAssetsByModeAndView(String brand, String country, Guid publicationID, Guid objectID, String mode, String view, IEnumerable<Asset> assets)
        {
            return await _assetService.PutAssetsByModeAndView(brand, country, publicationID, objectID, mode, view, assets);
        }

        private async Task<Result> PublishDefaultAssets(String brand, String country, Guid publicationID, Guid objectID, IEnumerable<Asset> assets)
        {
            var defaultAssets =
                assets.Where(a => String.IsNullOrEmpty(a.AssetType.Mode) || String.IsNullOrEmpty(a.AssetType.View))
                      .OrderBy(asset => asset.Name)
                      .ThenBy(asset => asset.AssetType.Name);
            return await _assetService.PutDefaultAssets(brand, country, publicationID, objectID, defaultAssets);
        }
    }
}