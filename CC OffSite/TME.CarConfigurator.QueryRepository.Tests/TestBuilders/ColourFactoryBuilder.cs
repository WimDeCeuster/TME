using FakeItEasy;
using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
{
    public class ColourFactoryBuilder
    {
        private IColourService _colourService = A.Fake<IColourService>();
        private IAssetFactory _assetFactory = A.Fake<IAssetFactory>();

        public ColourFactoryBuilder WithColourService(IColourService colourService)
        {
            _colourService = colourService;

            return this;
        }

        public ColourFactoryBuilder WithAssetFactory(IAssetFactory assetFactory)
        {
            _assetFactory = assetFactory;

            return this;
        }

        public IColourFactory Build()
        {
            return new ColourFactory(_colourService, _assetFactory);
        }
    }
}