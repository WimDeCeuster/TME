using FakeItEasy;
using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
{
    public class TransmissionFactoryBuilder
    {
        private ITransmissionService _transmissionService = A.Fake<ITransmissionService>();

        public TransmissionFactoryBuilder WithTransmissionService(ITransmissionService transmissionService)
        {
            _transmissionService = transmissionService;

            return this;
        }

        public ITransmissionFactory Build()
        {
            return new TransmissionFactory(_transmissionService);
        }
    }
}