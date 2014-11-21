using System;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.S3.CommandServices;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Tests.Shared;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAS3ColourCombinationPublisher
{
    public class WhenPublishingGenerationColourCombinationsForACar : TestBase
    {
        private IColourPublisher _colourPublisher;
        private IContext _context;
        private IService _s3Service;

        protected override void Arrange()
        {
            _context = new ContextBuilder().Build();

            _s3Service = A.Fake<IService>();

            var serialiser = A.Fake<ISerialiser>();
            var keymanager = A.Fake<IKeyManager>();

            var colourService = new ColourCombinationService(_s3Service, serialiser, keymanager);
            _colourPublisher = new ColourCombinationPublisherBuilder().WithService(colourService).Build();
        }

        protected override void Act()
        {
            _colourPublisher.PublishCarColourCombinations(_context);
        }

        [Fact]
        public void ThenTheColourCombinationsOfACarShouldBePublished()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<String>._, A<String>._, A<String>._, A<String>._)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}