using System;
using System.Collections.Generic;
using FakeItEasy;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.S3.CommandServices;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAS3ColourPublisher
{
    public class WhenPublishingGenerationColourCombinationsForACar : TestBase
    {
        private IColourPublisher _colourPublisher;
        private IContext _context;
        private IService _s3Service;
        private const string LANGUAGE1 = "de";
        private const string COLOUR_COMBINATIONS_FOR_CAR = "the key for car";
        private const string SERIALISEDDATA = "serialised carcolourcombinations data";

        protected override void Arrange()
        {
            var carID = Guid.NewGuid();

            var publication = new PublicationBuilder().WithID(Guid.NewGuid()).Build();

            var repoColourCombination = new ColourCombinationBuilder().WithId(Guid.NewGuid()).Build();

            _context = new ContextBuilder()
                .WithLanguages(LANGUAGE1)
                .WithPublication(LANGUAGE1,publication)
                .AddCarColourCombination(LANGUAGE1,carID,repoColourCombination)
                .Build();

            _s3Service = A.Fake<IService>();

            var serialiser = A.Fake<ISerialiser>();
            A.CallTo(() => serialiser.Serialise(A<IList<ColourCombination>>._)).Returns(SERIALISEDDATA);

            var keymanager = A.Fake<IKeyManager>();
            A.CallTo(() => keymanager.GetCarColourCombinationsKey(publication.ID, carID))
                .Returns(COLOUR_COMBINATIONS_FOR_CAR);

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

        [Fact]
        public void ThenCarColourCombinationsShouldBePutForCar()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<String>._, A<String>._, COLOUR_COMBINATIONS_FOR_CAR, SERIALISEDDATA)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}