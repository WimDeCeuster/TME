using FakeItEasy;
using TME.CarConfigurator.QueryRepository.Interfaces;
using TME.CarConfigurator.QueryRepository.S3;
using TME.CarConfigurator.QueryRepository.Service.Interfaces;

namespace TME.CarConfigurator.QueryRepository.Tests.TestBuilders.S3
{
    internal class PublicationRepositoryBuilder
    {
        private readonly IPublicationRepository _publicationRepository;
        private IPublicationService _publicationService = PublicationServiceBuilder.InitializeFakeService().Build();

        private PublicationRepositoryBuilder(IPublicationRepository publicationRepository)
        {
            _publicationRepository = publicationRepository;
        }

        public static PublicationRepositoryBuilder Initialize()
        {
            return new PublicationRepositoryBuilder(null);
        }

        public static PublicationRepositoryBuilder InitializeFakeRepository()
        {
            var repository = A.Fake<IPublicationRepository>();

            return new PublicationRepositoryBuilder(repository);
        }

        public IPublicationRepository Build()
        {
            return _publicationRepository ?? new PublicationRepository(_publicationService);
        }

        public PublicationRepositoryBuilder WithPublicationService(IPublicationService publicationService)
        {
            _publicationService = publicationService;

            return this;
        }
    }
}