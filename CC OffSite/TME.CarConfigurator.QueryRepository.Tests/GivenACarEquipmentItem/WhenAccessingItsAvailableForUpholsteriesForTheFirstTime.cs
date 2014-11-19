using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class WhenAccessingItsAvailableForUpholsteriesForTheFirstTime : TestBase
    {
        ICarEquipmentItem _carEquipmentItem;
        IEnumerable<IUpholsteryInfo> _upholsteryInfos;
        Repository.Objects.Colours.UpholsteryInfo _upholsteryInfo1;
        Repository.Objects.Colours.UpholsteryInfo _upholsteryInfo2;

        protected override void Arrange()
        {
            _upholsteryInfo1 = new UpholsteryInfoBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _upholsteryInfo2 = new UpholsteryInfoBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoCarEquipmentItem = new CarOptionBuilder()
                .WithAvailableForUpholsteries(_upholsteryInfo1, _upholsteryInfo2)
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
        }

        protected override void Act()
        {
            _upholsteryInfos = _carEquipmentItem.AvailableForUpholsteries;
        }

        [Fact]
        public void ThenItShouldHaveTheItems()
        {
            _upholsteryInfos.Count().Should().Be(2);

            _upholsteryInfos.Should().Contain(upholsteryInfo => upholsteryInfo.ID == _upholsteryInfo1.ID);
            _upholsteryInfos.Should().Contain(upholsteryInfo => upholsteryInfo.ID == _upholsteryInfo2.ID);
        }
    }
}
