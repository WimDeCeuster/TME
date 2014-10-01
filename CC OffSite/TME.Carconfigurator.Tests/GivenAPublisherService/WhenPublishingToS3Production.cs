using FakeItEasy;
using FluentAssertions;
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
        PublicationDataSubset _dataSubset;
        PublicationService _service;
        IContextFactory _contextFactory;
        IPublisher _publisher;
        IContext _context;

        protected override void Arrange()
        {
            _publisherFactory = A.Fake<IPublisherFactory>();
            _target = PublicationTarget.S3;
            _dataSubset = PublicationDataSubset.Live;
            _environment = PublicationEnvironment.Production;
            _context = A.Fake<IContext>();
            _contextFactory = A.Fake<IContextFactory>();
            _publisher = A.Fake<IPublisher>();

            A.CallTo(() => _publisherFactory.Get(_target, _environment)).Returns(_publisher);
            A.CallTo(() => _contextFactory.Get(Guid.Empty, _dataSubset)).Returns(_context);

            _service = new PublicationService(_contextFactory, _publisherFactory, A.Fake<IMapper>());
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

        [Fact]
        public void ThenContextFactoryShouldBeCalledWithCorrectParamaters()
        {
            A.CallTo(() => _contextFactory.Get(Guid.Empty, _dataSubset)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenACallToPublisherPublishHappens()
        {
            A.CallTo(() => _publisher.Publish(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
