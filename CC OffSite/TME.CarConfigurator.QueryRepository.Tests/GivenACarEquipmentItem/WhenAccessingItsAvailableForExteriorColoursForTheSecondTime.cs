using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenACarEquipmentItem
{
    public class WhenAccessingItsAvailableForExteriorColoursForTheSecondTime : TestBase
    {
        ICarEquipmentItem _carEquipmentItem;
        IEnumerable<IExteriorColourInfo> _secondExteriorColourInfos;
        IEnumerable<IExteriorColourInfo> _firstExteriorColourInfos;
        Repository.Objects.Colours.ExteriorColourInfo _exteriorColourInfo1;
        Repository.Objects.Colours.ExteriorColourInfo _exteriorColourInfo2;

        protected override void Arrange()
        {
            _exteriorColourInfo1 = new ExteriorColourInfoBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _exteriorColourInfo2 = new ExteriorColourInfoBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoCarEquipmentItem = new CarOptionBuilder()
                .WithAvailableForExteriorColours(_exteriorColourInfo1, _exteriorColourInfo2)
                .Build();

            var repoCarEquipment = new CarEquipmentBuilder()
                .WithOptions(repoCarEquipmentItem)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .Build();

            var context = new ContextBuilder().Build();

            var equipmentService = A.Fake<IEquipmentService>();
            A.CallTo(() => equipmentService.GetCarEquipment(A<Guid>._, A<Guid>._, A<Context>._)).Returns(repoCarEquipment);

            var equipmentFactory = new EquipmentFactoryBuilder()
                .WithEquipmentService(equipmentService)
                .Build();

            _carEquipmentItem = equipmentFactory.GetCarEquipment(Guid.Empty, publication, context).Options.Single();

            _firstExteriorColourInfos = _carEquipmentItem.AvailableForExteriorColours;
        }

        protected override void Act()
        {
            _secondExteriorColourInfos = _carEquipmentItem.AvailableForExteriorColours;
        }

        [Fact]
        public void ThenItShouldNotRecalculateTheItems()
        {
            _secondExteriorColourInfos.Should().BeSameAs(_firstExteriorColourInfos);
        }

        [Fact]
        public void ThenItShouldHaveTheItems()
        {
            _secondExteriorColourInfos.Count().Should().Be(2);

            _secondExteriorColourInfos.Should().Contain(exteriorColourInfo => exteriorColourInfo.ID == _exteriorColourInfo1.ID);
            _secondExteriorColourInfos.Should().Contain(exteriorColourInfo => exteriorColourInfo.ID == _exteriorColourInfo2.ID);
        }
    }
}
