using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenACarPackExteriorColourType
{
    public class WhenAccessingItsColourCombinationsTheSecondTime : TestBase
    {
        ICarPackExteriorColourType _carPackExteriorColourType;
        List<IColourCombinationInfo> _firstColourCombinations;
        List<IColourCombinationInfo> _secondColourCombinations;
        List<ColourCombinationInfo> _repoColourCombinations;

        protected override void Arrange()
        {
            var info1 = new ColourCombinationInfoBuilder()
                .WithExteriorColour(new ExteriorColourInfoBuilder().WithId(Guid.NewGuid()).Build())
                .WithUpholstery(new UpholsteryInfoBuilder().WithId(Guid.NewGuid()).Build())
                .Build();

            var info2 = new ColourCombinationInfoBuilder()
                .WithExteriorColour(new ExteriorColourInfoBuilder().WithId(Guid.NewGuid()).Build())
                .WithUpholstery(new UpholsteryInfoBuilder().WithId(Guid.NewGuid()).Build())
                .Build();

            _repoColourCombinations = new List<ColourCombinationInfo> { info1, info2 };

            var repoCarPackExteriorColourType = new CarPackExteriorColourTypeBuilder()
                .WithColourCombinations(info1, info2)
                .Build();

            var repoCarPackEquipment = new CarPackEquipmentBuilder()
                .WithExteriorColourTypes(repoCarPackExteriorColourType)
                .Build();

            var repoCarPack = new CarPackBuilder()
                .WithCarPackEquipment(repoCarPackEquipment)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .Build();

            var context = new ContextBuilder().Build();

            var packService = A.Fake<IPackService>();
            A.CallTo(() => packService.GetCarPacks(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new[] { repoCarPack });

            var equipmentFactory = new EquipmentFactoryBuilder().Build();

            var packFactory = new PackFactoryBuilder()
                .WithPackService(packService)
                .WithEquipmentFactory(equipmentFactory)
                .Build();

            var carPack = packFactory.GetCarPacks(publication, context, Guid.Empty).Single();
            _carPackExteriorColourType = carPack.Equipment.ExteriorColourTypes.Single();

            _firstColourCombinations = _carPackExteriorColourType.ColourCombinations.ToList();
        }

        protected override void Act()
        {
            _secondColourCombinations = _carPackExteriorColourType.ColourCombinations.ToList();
        }

        [Fact]
        public void ThenItShouldNotRecalculateTheColourCombinations()
        {
            _secondColourCombinations.Should().BeEquivalentTo(_firstColourCombinations);
        }

        [Fact]
        public void ThenTheColourCombinationsShouldBeCorrect()
        {
            _secondColourCombinations.Count.ShouldBeEquivalentTo(2);

            _secondColourCombinations.First().ExteriorColour.ID.ShouldBeEquivalentTo(_repoColourCombinations.First().ExteriorColour.ID);
            _secondColourCombinations.Last().Upholstery.ID.ShouldBeEquivalentTo(_repoColourCombinations.Last().Upholstery.ID);
        }
    }
}
