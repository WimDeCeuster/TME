using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
{
    public class AssetFactoryBuilder
    {
        private IAssetService _assetService;

        public AssetFactoryBuilder WithAssetService(IAssetService assetService)
        {
            _assetService = assetService;

            return this;
        }

        public IAssetFactory Build()
        {
            return new AssetFactory(_assetService);
        }
    }
}