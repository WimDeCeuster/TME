using FakeItEasy;
using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
{
    public class EngineFactoryBuilder
    {
        private IEngineService _engineService = A.Fake<IEngineService>();
        private IAssetFactory _assetFactory = A.Fake<IAssetFactory>();

        public EngineFactoryBuilder WithEngineService(IEngineService engineService)
        {
            _engineService = engineService;

            return this;
        }

        public EngineFactoryBuilder WithAssetFactory(IAssetFactory assetFactory)
        {
            _assetFactory = assetFactory;

            return this;
        }

        public IEngineFactory Build()
        {
            return new EngineFactory(_engineService, _assetFactory);
        }
    }
}