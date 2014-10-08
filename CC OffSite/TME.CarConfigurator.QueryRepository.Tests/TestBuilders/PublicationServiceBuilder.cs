using FakeItEasy;
using TME.CarConfigurator.QueryRepository.Service;
using TME.CarConfigurator.QueryRepository.Service.Interfaces;
using TME.CarConfigurator.QueryRepository.Tests.TestBuilders.Base;

namespace TME.CarConfigurator.QueryRepository.Tests.TestBuilders
{
    public class PublicationServiceBuilder : ServiceBuilderBase<IPublicationService>
    {
        private readonly IPublicationService _service;

        private PublicationServiceBuilder(IPublicationService service)
        {
            _service = service;
        }

        public static PublicationServiceBuilder InitializeFakeService()
        {
            var service = A.Fake<IPublicationService>();

            return new PublicationServiceBuilder(service);
        }

        public static PublicationServiceBuilder Initialize()
        {
            return new PublicationServiceBuilder(null);
        }

        public override IPublicationService Build()
        {
            return _service ?? new PublicationService(Serialiser, Service, KeyManager);
        }
    }
}