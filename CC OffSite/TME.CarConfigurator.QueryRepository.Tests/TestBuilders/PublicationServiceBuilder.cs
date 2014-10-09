using FakeItEasy;
using TME.CarConfigurator.Query.Tests.TestBuilders.Base;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.S3.QueryServices;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
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