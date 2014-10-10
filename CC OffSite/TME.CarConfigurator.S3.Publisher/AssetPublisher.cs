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
        private IAssetService _assetService;

        public async Task<IEnumerable<Result>> PublishAssets(IContext context)
        {
            var tasks = new List<Task<IEnumerable<Result>>>();

            foreach (var entry in context.ContextData)
            {
                var data = entry.Value;

                tasks.Add(PutGenerationsAssets(context.Brand, context.Country, data));
            }

            var result = await Task.WhenAll(tasks);
            return result.SelectMany(xs => xs);
        }

        async Task<IEnumerable<Result>> PutGenerationsAssets(string brand, string country, ContextData data)
        {
            var publication = data.Publication;

            var tasks = new List<Task<Result>>();
            var assets = publication.Generation.Assets.ToDictionary(key => key.ID, val => val.Name);

            foreach (var asset in assets)
            {
                tasks.Add(_assetService.PutGenerationAsset(brand,country,publication.ID,asset.Key,asset.Value));
            }

            return await Task.WhenAll(tasks);
        }
    }
}