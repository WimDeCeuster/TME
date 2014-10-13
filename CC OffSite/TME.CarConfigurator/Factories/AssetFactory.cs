using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;

namespace TME.CarConfigurator.Factories
{
    public class AssetFactory : IAssetFactory
    {
        public IEnumerable<IAsset> CreateAssets(IEnumerable<Repository.Objects.Assets.Asset> repoAssets)
        {
            return repoAssets.Select(a => new Asset(a));
        }
    }
}