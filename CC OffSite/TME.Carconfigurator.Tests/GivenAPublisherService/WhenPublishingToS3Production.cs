using FakeItEasy;
using System;
using TME.CarConfigurator.Publisher;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAPublisherService
{
    public class WhenPublishingToS3Production : Base.TestBase
    {
        IPublisherFactory _publisherFactory;
        PublicationTarget _target;
        PublicationEnvironment _environment;
        PublicationService _service;

        protected override void Arrange()
        {
            _publisherFactory = A.Fake<IPublisherFactory>();
            _target = PublicationTarget.S3;
            _environment = PublicationEnvironment.Production;

            _service = new PublicationService(A.Fake<IContextFactory>(), _publisherFactory, A.Fake<IMapper>());
        }

        protected override void Act()
        {
            _service.Publish(Guid.Empty, _target, _environment, PublicationDataSubset.Live);
        }

        [Fact]
        public void ThenPublisherFactoryShouldBeCalledWithCorrectParamaters()
        {
            A.CallTo(() => _publisherFactory.Get(_target, _environment)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
