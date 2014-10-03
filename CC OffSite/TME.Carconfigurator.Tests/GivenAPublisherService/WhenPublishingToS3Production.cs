using FakeItEasy;
using System;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Publisher.Enums;
using TME.CarConfigurator.Publisher.Interfaces;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAPublisherService
{
    public class WhenPublishingToS3Production : Base.TestBase
    {
        IPublisherFactory _publisherFactory;
        String _target;
        PublicationDataSubset _dataSubset;
        PublicationService _service;
        IContextFactory _contextFactory;
        IPublisher _publisher;
        IContext _context;
        ICarDbModelGenerationFinder _generationFinder;
        String _brand = "Toyota";
        String _country = "BE";
        IMapper _mapper;

        protected override void Arrange()
        {
            _publisherFactory = A.Fake<IPublisherFactory>();
            _target = "S3";
            _dataSubset = PublicationDataSubset.Live;
            _context = A.Fake<IContext>();
            _contextFactory = A.Fake<IContextFactory>();
            _publisher = A.Fake<IPublisher>();
            _generationFinder = A.Fake<ICarDbModelGenerationFinder>();
            _mapper = A.Fake<IMapper>();
            

            A.CallTo(() => _publisherFactory.Get(_target)).Returns(_publisher);
            A.CallTo(() => _contextFactory.Get(_brand, _country, Guid.Empty, _dataSubset)).Returns(_context);


            _service = new PublicationService(_contextFactory, _publisherFactory, _mapper, _generationFinder);
        }

        protected override void Act()
        {
            _service.Publish(Guid.Empty, _target, _brand, _country, PublicationDataSubset.Live);
        }

        [Fact]
        public void ThenPublisherFactoryShouldBeCalledWithCorrectParamaters()
        {
            A.CallTo(() => _publisherFactory.Get(_target)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenContextFactoryShouldBeCalledWithCorrectParamaters()
        {
            A.CallTo(() => _contextFactory.Get(_brand, _country, Guid.Empty, _dataSubset)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenACallToPublisherPublishHappens()
        {
            A.CallTo(() => _publisher.Publish(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
