using FakeItEasy;
using TME.CarConfigurator.QueryRepository.Interfaces;
using TME.CarConfigurator.S3.QueryServices.Interfaces;

namespace TME.CarConfigurator.QueryRepository.Tests.TestBuilders
{
    internal class S3PublicationRepositoryBuilder
    {
        private readonly IPublicationRepository _publicationRepository;
        private IPublicationService _publicationService = PublicationServiceBuilder.InitializeFakeService().Build();

        private S3PublicationRepositoryBuilder(IPublicationRepository publicationRepository)
        {
            _publicationRepository = publicationRepository;
        }

        public static S3PublicationRepositoryBuilder Initialize()
        {
            return new S3PublicationRepositoryBuilder(null);
        }

        public static S3PublicationRepositoryBuilder InitializeFakeRepository()
        {
            var repository = A.Fake<IPublicationRepository>();

            return new S3PublicationRepositoryBuilder(repository);
        }

        public IPublicationRepository Build()
        {
            return _publicationRepository ?? new PublicationRepository(_publicationService);
        }

        public S3PublicationRepositoryBuilder WithPublicationService(IPublicationService publicationService)
        {
            _publicationService = publicationService;

            return this;
        }
    }
}