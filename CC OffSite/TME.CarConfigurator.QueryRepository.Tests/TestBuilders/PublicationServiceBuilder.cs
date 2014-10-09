using FakeItEasy;
using TME.CarConfigurator.QueryRepository.Tests.TestBuilders.Base;
using TME.CarConfigurator.S3.QueryServices;
using TME.CarConfigurator.S3.QueryServices.Interfaces;

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