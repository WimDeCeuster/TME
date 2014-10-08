using System;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.QueryRepository.Interfaces;
using TME.CarConfigurator.QueryRepository.Tests.TestBuilders;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders.RepositoryObjects;
using Xunit;

namespace TME.CarConfigurator.QueryRepository.Tests.GivenAnS3PublicationRepository
{
    public class WhenGetPublicationIsCalled : TestBase
    {
        private IPublicationRepository _publicationRepository;
        private Publication _actualPublication;
        private Publication _expectedPublication;
        private Guid _publicationId;

        protected override void Arrange()
        {
            _publicationId = Guid.NewGuid();
            _expectedPublication = PublicationBuilder.Initialize().WithID(_publicationId).Build();

            var publicationService = PublicationServiceBuilder.InitializeFakeService().Build();

            A.CallTo(() => publicationService.GetPublication(A<Guid>._)).ReturnsLazily(() => PublicationBuilder.Initialize().WithID(Guid.NewGuid()).Build());
            A.CallTo(() => publicationService.GetPublication(_publicationId)).Returns(_expectedPublication);

            _publicationRepository = S3PublicationRepositoryBuilder.Initialize().WithPublicationService(publicationService).Build();
        }

        protected override void Act()
        {
            _actualPublication = _publicationRepository.GetPublication(_publicationId);
        }

        [Fact]
        public void ThenItShouldReturnTheCorrectPublication()
        {
            _actualPublication.Should().Be(_expectedPublication);
        }
    }
}