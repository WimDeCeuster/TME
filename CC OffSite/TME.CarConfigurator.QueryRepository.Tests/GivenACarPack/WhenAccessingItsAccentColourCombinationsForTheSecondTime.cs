using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Packs;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenACarPack
{
    public class WhenAccessingItsAccentColourCombinationsForTheSecondTime : TestBase
    {
        private IReadOnlyList<ICarPack> _carPack;
        private IReadOnlyList<IAccentColourCombination> _secondPackAccentColourCombinations;
        private IColourService _colourService;
        private AccentColourCombination _repoAccentColourCombination;
        private Guid _carID;
        private Publication _publication;
        private IReadOnlyList<IAccentColourCombination> _firstPackAccentColourCombinations;

        protected override void Arrange()
        {
            _repoAccentColourCombination = new AccentColourCombinationBuilder()
                .WithBodyColour(new EquipmentExteriorColourBuilder()
                .WithId(Guid.NewGuid()).Build()).Build();

            var repoCarPack = new CarPackBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _carID = Guid.NewGuid();

            _publication = new PublicationBuilder().WithID(Guid.NewGuid()).Build();

            var context = new ContextBuilder().Build();

            var packService = A.Fake<IPackService>();
            A.CallTo(() => packService.GetCarPacks(_publication.ID, _carID, context)).Returns(new[] { repoCarPack });

            _colourService = A.Fake<IColourService>();
            A.CallTo(() => _colourService.GetCarPackAccentColourCombinations(_carID, _publication.ID, context))
                .Returns(new Dictionary<Guid, IEnumerable<AccentColourCombination>> { { repoCarPack.ID, new[] { _repoAccentColourCombination } } });

            var colourFactory = new ColourFactoryBuilder()
                .WithColourService(_colourService).Build();

            var packFactory = new PackFactoryBuilder()
                .WithPackService(packService)
                .WithColourFactory(colourFactory)
                .Build();

            _carPack = packFactory.GetCarPacks(_publication, context, _carID);

            _firstPackAccentColourCombinations = _carPack.First().AccentColourCombinations;
        }

        protected override void Act()
        {
            _secondPackAccentColourCombinations = _carPack.First().AccentColourCombinations;
        }

        [Fact]
        public void ThenItShouldNotCalculateThePackAccentColourCombinations()
        {
            A.CallTo(() => _colourService.GetCarPackAccentColourCombinations(_carID, _publication.ID, A<Context>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldHaveTheCorrectColourCombinations()
        {
            _secondPackAccentColourCombinations.Should().BeSameAs(_firstPackAccentColourCombinations);

            _secondPackAccentColourCombinations.Should()
                .Contain(cc => cc.BodyColour.ID == _repoAccentColourCombination.BodyColour.ID);
        }
    }
}