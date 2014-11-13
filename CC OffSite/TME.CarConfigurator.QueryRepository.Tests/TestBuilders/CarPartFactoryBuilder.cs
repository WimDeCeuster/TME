using FakeItEasy;
using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
{
    public class CarPartFactoryBuilder
    {
        private ICarPartService _carPartService = A.Fake<ICarPartService>();

        private IAssetFactory _assetFactory = A.Fake<IAssetFactory>();

        public CarPartFactoryBuilder WithAssetFactory(IAssetFactory assetFactory)
        {
            _assetFactory = assetFactory;
            return this;
        }

        public CarPartFactoryBuilder WithCarPartService(ICarPartService carPartService)
        {
            _carPartService = carPartService;
            return this;
        }

        public ICarPartFactory Build()
        {
            return new CarPartFactory(_carPartService,_assetFactory);
        }
    }
}