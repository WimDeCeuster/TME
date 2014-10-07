using FakeItEasy;
using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Factories.Interfaces;
using TME.CarConfigurator.QueryRepository.Interfaces;
using TME.CarConfigurator.QueryRepository.Tests.TestBuilders.S3;

namespace TME.CarConfigurator.QueryRepository.Tests.TestBuilders
{
    internal class PublicationFactoryBuilder
    {
        private readonly IPublicationFactory _publicationFactory;
        private IPublicationRepository _publicationRepository = PublicationRepositoryBuilder.InitializeFakeRepository().Build();

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

        public PublicationFactoryBuilder WithPublicationRepository(IPublicationRepository publicationRepository)
        {
            _publicationRepository = publicationRepository;

            return this;
        }

        public IPublicationFactory Build()
        {
            return _publicationFactory ?? new PublicationFactory(_publicationRepository);
        }
    }
}