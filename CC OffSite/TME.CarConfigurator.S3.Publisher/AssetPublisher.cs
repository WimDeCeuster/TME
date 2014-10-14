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
            var tasks = new List<Task<IEnumerable<Result>>>();

            foreach (var entry in context.ContextData)
            {
                var data = entry.Value;
                tasks.Add(PutGenerationsAssets(context.Brand, context.Country, data));
            }

            var result = await Task.WhenAll(tasks);
            return result.SelectMany(xs => xs.ToList());
        }

        async Task<IEnumerable<Result>> PutGenerationsAssets(String brand, String country, ContextData data)
        {
            var generationAssets = data.BodyTypeAssets;


            var groupAssetsPerId = new Dictionary<Guid,Dictionary<Guid,IEnumerable<Asset>>>();

            Task<Result> task = null;
            foreach (var bodyType in data.GenerationBodyTypes)
            {
                var assets = generationAssets.ToDictionary(
                key => key.ID, 
                val => generationAssets.Where(genAsset => genAsset.ID.Equals(val.ID)));

                groupAssetsPerId.Add(bodyType.ID,assets );
                task = _assetService.PutGenerationsAsset(brand, country, data.Publication.ID,bodyType.ID, assets);
            }

            return await Task.WhenAll(task);
        }
    }
}