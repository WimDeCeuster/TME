using FakeItEasy;
using TME.CarConfigurator.QueryRepository.Interfaces;

namespace TME.CarConfigurator.QueryRepository.Tests.TestBuilders
{
    internal class PublicationRepositoryBuilder
    {
        private readonly IPublicationRepository _publicationRepository;

        private PublicationRepositoryBuilder(IPublicationRepository publicationRepository)
        {
            _publicationRepository = publicationRepository;
        }

        public static PublicationRepositoryBuilder InitializeFakeRepository()
        {
            var repository = A.Fake<IPublicationRepository>();

            return new PublicationRepositoryBuilder(repository);
        }

        public IPublicationRepository Build()
        {
            return _publicationRepository;
        }
    }
}