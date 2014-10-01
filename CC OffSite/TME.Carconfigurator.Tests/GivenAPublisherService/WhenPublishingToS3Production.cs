using FakeItEasy;
using FluentAssertions;
using System;
using TME.CarConfigurator.Publisher;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAPublisherService
{
    public class WhenPublishingToS3Production
    {
        IPublisherFactory _publisherFactory;
        PublicationTarget _target;
        PublicationEnvironment _environment;

        public WhenPublishingToS3Production()
        {
            // Arrange
            _publisherFactory = A.Fake<IPublisherFactory>();
            _target = PublicationTarget.S3;
            _environment = PublicationEnvironment.Production;

            var service = new PublicationService(A.Fake<IContextFactory>(), _publisherFactory, A.Fake<IMapper>());

            // Act
            service.Publish(Guid.Empty, _target, _environment, PublicationDataSubset.Live);
        }

        [Fact]
        public void Then()
        {
            A.CallTo(() => _publisherFactory.Get(_target, _environment)).MustHaveHappened(Repeated.Exactly.Once);
        }

    }
}
