﻿using FakeItEasy;
using System;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Publisher.Common;
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
        IPublisher _publisher;
        IMapper _mapper;
        private Guid _generationId = Guid.NewGuid();

        protected override void Arrange()
        {
            _publisherFactory = A.Fake<IPublisherFactory>(opt => opt.Strict());
            _publisher = A.Fake<IPublisher>();
            _mapper = A.Fake<IMapper>();
            
            A.CallTo(() => _publisherFactory.GetPublisher(Target, Environment, DataSubset)).Returns(_publisher);
            
            _carConfiguratorPublisher = new CarConfiguratorPublisher(_publisherFactory, _mapper);
        }

        protected override void Act()
        {
            var result = _carConfiguratorPublisher.PublishAsync(_generationId, Environment, Target, Brand, Country, DataSubset).Result;
        }

        [Fact]
        public void ThenACallToPublisherPublishHappens()
        {
            A.CallTo(() => _publisher.PublishAsync(A<Context>.That.Matches(c => c.Brand == Brand && c.Country == Country &&c.GenerationID == _generationId)))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenACallToMapperHappens()
        {
            A.CallTo(() => _mapper.MapAsync(A<Context>.That.Matches(c => c.Brand == Brand && c.Country == Country && c.GenerationID == _generationId)))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
