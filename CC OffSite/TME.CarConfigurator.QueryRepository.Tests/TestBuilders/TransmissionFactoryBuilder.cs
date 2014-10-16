using FakeItEasy;
using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
{
    public class TransmissionFactoryBuilder
    {
        private ITransmissionService _transmissionService = A.Fake<ITransmissionService>();
        private IAssetFactory _assetFactory = A.Fake<IAssetFactory>();

        public TransmissionFactoryBuilder WithTransmissionService(ITransmissionService transmissionService)
        {
            _transmissionService = transmissionService;

            return this;
        }
        
        public TransmissionFactoryBuilder WithAssetService(IAssetFactory assetFactory)
        {
            _assetFactory = assetFactory;

            return this;
        }

        public ITransmissionFactory Build()
        {
            return new TransmissionFactory(_transmissionService,_assetFactory);
        }
    }
}