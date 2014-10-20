using FakeItEasy;
using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
{
    public class SubModelFactoryBuilder
    {
        private ISubModelService _subModelService = A.Fake<ISubModelService>();
        private IAssetFactory _assetFactory = A.Fake<IAssetFactory>();

        public SubModelFactoryBuilder WithSubModelService(ISubModelService subModelService)
        {
            _subModelService = subModelService;
            return this;
        }

        public SubModelFactoryBuilder WithAssetFactory(IAssetFactory assetFactory)
        {
            _assetFactory = assetFactory;
            return this;
        }

        public ISubModelFactory Build()
        {
            return new SubModelFactory(_subModelService,_assetFactory);
        }
    }
}