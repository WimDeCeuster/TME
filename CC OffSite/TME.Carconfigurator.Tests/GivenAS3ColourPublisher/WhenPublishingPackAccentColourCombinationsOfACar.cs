using System;
using System.Collections.Generic;
using FakeItEasy;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.Repository.Objects.Packs;
using TME.CarConfigurator.S3.CommandServices;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAS3ColourPublisher
{
    public class WhenPublishingPackAccentColourCombinationsOfACar : TestBase    
    {
        private IContext _context;
        private IColourPublisher _colourPublisher;
        private IService _service;
        private const string CarPackAccentColourCombinationKey = "The pack accent colourcombination Key";
        private const string SerialisedData = "The Serialised PackAccentColour";
        private const string Language1 = "de";

        protected override void Arrange()
        {
            var packID = Guid.NewGuid();
            var carID = Guid.NewGuid();

            var repoAccentColourCombination = new AccentColourCombinationBuilder()
                .WithBodyColour(A.Fake<ExteriorColour>())
                .WithPrimaryColour(A.Fake<ExteriorColour>())
                .WithSecondaryColour(A.Fake<ExteriorColour>())
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .Build();

            _context = new ContextBuilder()
                .WithLanguages(Language1)
                .WithPublication(Language1, publication)
                .AddCarPackAccentColourCombinations(Language1, carID, packID, repoAccentColourCombination)
                .Build();

            _service = A.Fake<IService>();
            var keymanager = A.Fake<IKeyManager>();
            A.CallTo(() => keymanager.GetCarPackAccentColourCombinationsKey(publication.ID, carID))
                .Returns(CarPackAccentColourCombinationKey);

            var serialiser = A.Fake<ISerialiser>();
            A.CallTo(() => serialiser.Serialise(A<IDictionary<Guid, List<AccentColourCombination>>>._)).Returns(SerialisedData);

            var colourCombinationService = new ColourService(_service, serialiser, keymanager);

            _colourPublisher = new ColourPublisherBuilder().WithService(colourCombinationService).Build();
        }

        protected override void Act()
        {
            _colourPublisher.PublishCarPackAccentColourCombinations(_context);
        }

        [Fact]
        public void ThenPackAccentColourCombinationsArePublishedForACar()
        {
            A.CallTo(() => _service.PutObjectAsync(A<String>._, A<String>._, CarPackAccentColourCombinationKey, SerialisedData)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}