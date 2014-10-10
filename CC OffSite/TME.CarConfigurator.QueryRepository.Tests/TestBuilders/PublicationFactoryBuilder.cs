using FakeItEasy;
using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
{
    internal class PublicationFactoryBuilder
    {
        private readonly IPublicationFactory _publicationFactory;
        private IPublicationService _publicationRepository = PublicationServiceBuilder.InitializeFakeService().Build();

        private PublicationFactoryBuilder()
        {

        }

        private PublicationFactoryBuilder(IPublicationFactory factory)
        {
            _publicationFactory = factory;
        }

        public static PublicationFactoryBuilder Initialize()
        {
            return new PublicationFactoryBuilder();
        }

        public static PublicationFactoryBuilder InitializeFakeFactory()
        {
            var factory = A.Fake<IPublicationFactory>();

            return new PublicationFactoryBuilder(factory);
        }

        public PublicationFactoryBuilder WithPublicationService(IPublicationService publicationService)
        {
            _publicationRepository = publicationService;

            return this;
        }

        public IPublicationFactory Build()
        {
            return _publicationFactory ?? new PublicationFactory(_publicationRepository);
        }
    }
}