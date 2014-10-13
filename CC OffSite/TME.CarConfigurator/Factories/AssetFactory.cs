using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.QueryServices;

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

        public IEnumerable<IAsset> GetAssets(Guid objectId)
        {
            var repoAssets = _assetService.GetAssets(objectId);

            return repoAssets.Select(a => new Asset(a));
        }
    }
}