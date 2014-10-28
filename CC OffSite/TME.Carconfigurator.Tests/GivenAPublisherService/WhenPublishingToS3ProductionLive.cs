using FakeItEasy;
using System;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAPublisherService
{
    public class WhenPublishingToS3ProductionLive : TestBase
    {
        const String Environment = "Production";
        const String Brand = "Toyota";
        const String Country = "BE";
        const String Target = "S3";
        const PublicationDataSubset DataSubset = PublicationDataSubset.Live;
        IPublisherFactory _publisherFactory;
        ICarConfiguratorPublisher _carConfiguratorPublisher;
        IContextFactory _contextFactory;
        IPublisher _publisher;
        IContext _context;
        IMapper _mapper;

        protected override void Arrange()
        {
            _publisherFactory = A.Fake<IPublisherFactory>(opt => opt.Strict());
            _context = A.Fake<IContext>();
            _contextFactory = A.Fake<IContextFactory>();
            _publisher = A.Fake<IPublisher>();
            _mapper = A.Fake<IMapper>();
            
            A.CallTo(() => _publisherFactory.GetPublisher(Target, Environment, DataSubset)).Returns(_publisher);
            A.CallTo(() => _contextFactory.Get(Brand, Country, Guid.Empty, DataSubset)).Returns(_context);
            
            _carConfiguratorPublisher = new CarConfiguratorPublisher(_contextFactory, _publisherFactory, _mapper);
        }

        protected override void Act()
        {
            var result = _carConfiguratorPublisher.PublishAsync(Guid.Empty, Environment, Target, Brand, Country, DataSubset).Result;
        }

        [Fact]
        public void ThenContextFactoryShouldBeCalledWithCorrectParamaters()
        {
            A.CallTo(() => _contextFactory.Get(Brand, Country, Guid.Empty, DataSubset)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenACallToPublisherPublishHappens()
        {
            A.CallTo(() => _publisher.PublishAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenACallToMapperHappens()
        {
            A.CallTo(() => _mapper.MapAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
