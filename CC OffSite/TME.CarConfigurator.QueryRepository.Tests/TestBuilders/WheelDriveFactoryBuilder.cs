using FakeItEasy;
using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
{
    public class WheelDriveFactoryBuilder
    {
        private IWheelDriveService _wheelDriveService = A.Fake<IWheelDriveService>();
        private IAssetFactory _assetFactory = A.Fake<IAssetFactory>();

        public WheelDriveFactoryBuilder WithWheelDriveService(IWheelDriveService wheelDriveService)
        {
            _wheelDriveService = wheelDriveService;

            return this;
        }

        public WheelDriveFactoryBuilder WithAssetFactory(IAssetFactory assetFactory)
        {
            _assetFactory = assetFactory;

            return this;
        }

        public IWheelDriveFactory Build()
        {
            return new WheelDriveFactory(_wheelDriveService, _assetFactory);
        }
    }
}