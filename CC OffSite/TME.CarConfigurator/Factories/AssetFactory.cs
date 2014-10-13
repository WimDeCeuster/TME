using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Factories
{
    public class AssetFactory : IAssetFactory
    {
        private readonly IAssetService _assetService;

        public AssetFactory(IAssetService assetService)
        {
            if (assetService == null) throw new ArgumentNullException("assetService");
            _assetService = assetService;
        }

        public IEnumerable<IAsset> GetAssets(Publication publication, Guid objectId, Context context)
        {
            var publicationTimeFrame = publication.GetCurrentTimeFrame();
            var repoAssets = _assetService.GetAssets(publication.ID, publicationTimeFrame.ID, objectId, context);

            return repoAssets.Select(a => new Asset(a));
        }
    }
}