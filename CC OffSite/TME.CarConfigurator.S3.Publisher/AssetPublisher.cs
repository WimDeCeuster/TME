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

        public async Task<IEnumerable<Result>> PublishAssets(IContext context)
        {
            var tasks = context.ContextData.Select(entry => entry.Value)
                .Select(data => PutGenerationsAssets(context.Brand, context.Country, data))
                .ToList();

            var result = await Task.WhenAll(tasks);
            return result.SelectMany(xs => xs.ToList());
        }

        async Task<IEnumerable<Result>> PutGenerationsAssets(String brand, String country, ContextData data)
        {
            var bodyTypeAssets = data.Assets;
            
            var groupAssetsByDefault = new Dictionary<Guid, IList<Asset>>();
            var groupAssetsByModeView = new Dictionary<Guid, IList<Asset>>();

            var tasks = new List<Task<Result>>();
            foreach (var bodyType in data.GenerationBodyTypes)
            {
                var nonDefaultBodyTypeAssets = new List<Asset>();
                var defaultBodyTypeAssets = new List<Asset>();

                foreach (var bodyTypeAsset in bodyTypeAssets.Values)
                {
                    foreach (var asset in bodyTypeAsset)
                    {
                        if (!String.IsNullOrEmpty(asset.AssetType.Mode) || !String.IsNullOrEmpty(asset.AssetType.View))
                            nonDefaultBodyTypeAssets.Add(asset);
                        else
                            defaultBodyTypeAssets.Add(asset);
                    }
                    
                }
                groupAssetsByDefault.Add(bodyType.ID,defaultBodyTypeAssets);
                groupAssetsByModeView.Add(bodyType.ID,nonDefaultBodyTypeAssets);

                tasks.Add(_assetService.PutDefaultBodyTypeAssets(brand, country, data.Publication.ID, bodyType.ID,groupAssetsByDefault));
                tasks.Add(_assetService.PutModeViewBodyTypeAssets(brand, country, data.Publication.ID, bodyType.ID, groupAssetsByModeView));
            }

            var result = await Task.WhenAll(tasks);
            return result.Select(xs => xs);
        }
    }
}