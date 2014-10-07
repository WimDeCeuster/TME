using FakeItEasy;
using TME.CarConfigurator.QueryRepository.Service.Interfaces;

namespace TME.CarConfigurator.QueryRepository.Tests.TestBuilders
{
    public class PublicationServiceBuilder
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

        public IPublicationService Build()
        {
            return _service;
        }
    }
}