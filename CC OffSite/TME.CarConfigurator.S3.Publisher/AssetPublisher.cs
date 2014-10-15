using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common;
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

        public Task<IEnumerable<Result>> PublishAssets(IContext context)
        {
            foreach (var languageCode in context.ContextData.Keys)
            {
                var publicationID = context.ContextData[languageCode].Publication.ID;
                PublishAssets(context.Brand, context.Country, publicationID, context.ContextData[languageCode].Assets);
            }
            return null;
        }

        private void PublishAssets(String brand, String country, Guid publicationID, Dictionary<Guid, List<Asset>> assetsPerObjectID)
        {
            foreach (var objectID in assetsPerObjectID.Keys)
            {
                PublishAssets(brand, country, publicationID, objectID, assetsPerObjectID[objectID]);
            }
        }

        private void PublishAssets(String brand, String country, Guid publicationID, Guid objectID, IList<Asset> assets)
        {
            PublishAssetsByModeAndView(brand, country, publicationID, objectID, assets);
            PublishDefaultAssets(brand, country, publicationID, objectID, assets);
        }

        private void PublishAssetsByModeAndView(String brand, String country, Guid publicationID, Guid objectID, IEnumerable<Asset> assets)
        {
            var assetsByModeAndView = assets.Where(
            a => !String.IsNullOrEmpty(a.AssetType.Mode) || !String.IsNullOrEmpty(a.AssetType.View))
            .GroupBy(a => new { a.AssetType.Mode, a.AssetType.View });

            foreach (var assetList in assetsByModeAndView)
            {
                var mode = assetList.Key.Mode;
                var view = assetList.Key.View;
                PublishAssetsByModeAndView(brand, country, publicationID, objectID, mode, view, assetList);
            }

        }

        private void PublishAssetsByModeAndView(String brand, String country, Guid publicationID, Guid objectID, String mode, String view, IEnumerable<Asset> assets)
        {
            _assetService.PutAssetsByModeAndView(brand, country, publicationID, objectID, mode, view, assets);
        }

        private void PublishDefaultAssets(String brand, String country, Guid publicationID, Guid objectID, IEnumerable<Asset> assets)
        {
            var defaultAssets =
                assets.Where(a => String.IsNullOrEmpty(a.AssetType.Mode) || String.IsNullOrEmpty(a.AssetType.View));
            _assetService.PutDefaultAssets(brand, country, publicationID, objectID, defaultAssets);
        }
    }
}