using FakeItEasy;
using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Query.Tests.GivenAGrade;
using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
{
    public class PackFactoryBuilder
    {
        private IPackService _packService;
        private IAssetFactory _assetFactory = A.Fake<IAssetFactory>();

        public PackFactoryBuilder WithPackService(IPackService packService)
        {
            _packService = packService;

            return this;
        }

        public PackFactoryBuilder WithAssetFactory(IAssetFactory assetFactory)
        {
            _assetFactory = assetFactory;

            return this;
        }

        public IPackFactory Build()
        {
            return new PackFactory(_packService, _assetFactory);
        }
    }
}