using FakeItEasy;
using System;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.S3.Shared.Interfaces;
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
        IPublisherFacadeFactory _publisherFacadeFactory;
        IPublisherFacade _publisherFacade;
        CarConfiguratorPublisher _publicationService;
        IContextFactory _contextFactory;
        IPublisher _publisher;
        IService _publisherService;
        IContext _context;
        ICarDbModelGenerationFinder _generationFinder;
        IMapper _mapper;

        protected override void Arrange()
        {
            _publisherFacadeFactory = A.Fake<IPublisherFacadeFactory>(opt => opt.Strict());
            _publisherFacade = A.Fake<IPublisherFacade>(opt => opt.Strict());
            _context = A.Fake<IContext>();
            _contextFactory = A.Fake<IContextFactory>();
            _publisherService = A.Fake<IService>();
            _publisher = A.Fake<IPublisher>();
            _generationFinder = A.Fake<ICarDbModelGenerationFinder>();
            _mapper = A.Fake<IMapper>();
            
            A.CallTo(() => _publisherFacadeFactory.GetFacade(Target)).Returns(_publisherFacade);
            A.CallTo(() => _publisherFacade.GetPublisher(Environment, DataSubset)).Returns(_publisher);
            A.CallTo(() => _contextFactory.Get(Brand, Country, Guid.Empty, DataSubset)).Returns(_context);
            
            _publicationService = new CarConfiguratorPublisher(_contextFactory, _publisherFacadeFactory, _mapper, _generationFinder);
        }

        protected override void Act()
        {
            _publicationService.Publish(Guid.Empty, Environment, Target, Brand, Country, DataSubset);
        }

        [Fact]
        public void ThenContextFactoryShouldBeCalledWithCorrectParamaters()
        {
            A.CallTo(() => _contextFactory.Get(Brand, Country, Guid.Empty, DataSubset)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenACallToPublisherPublishHappens()
        {
            A.CallTo(() => _publisher.Publish(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
