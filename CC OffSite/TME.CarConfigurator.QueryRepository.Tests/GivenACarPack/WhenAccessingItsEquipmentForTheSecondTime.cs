using System;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Core;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Repository.Objects.Packs;

namespace TME.CarConfigurator.Query.Tests.GivenACarPack
{
    public class WhenAccessingItsEquipmentTheSecondTime : TestBase
    {
        ICarPack _carPack;
        ICarPackEquipment _firstCarPackEquipment;
        ICarPackEquipment _secondCarPackEquipment;
        CarPackEquipment _repoCarPackEquipment;

        protected override void Arrange()
        {
            _repoCarPackEquipment = new CarPackEquipmentBuilder()
                .WithOptions(new CarPackOption(), new CarPackOption())
                .Build();

            var repoCarPack = new CarPackBuilder()
                .WithCarPackEquipment(_repoCarPackEquipment)
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

            _carPack = packFactory.GetCarPacks(publication, context, Guid.Empty).Single();

            _firstCarPackEquipment = _carPack.Equipment;
        }

        protected override void Act()
        {
            _secondCarPackEquipment = _carPack.Equipment ;
        }

        [Fact]
        public void ThenItShouldNotRecalculateTheCarPackEquipment()
        {
            _secondCarPackEquipment.Should().Be(_firstCarPackEquipment);
        }

        [Fact]
        public void ThenTheCarPackEquipmentShouldBeCorrect()
        {
            _secondCarPackEquipment.Accessories.Count.Should().Be(_repoCarPackEquipment.Accessories.Count);
        }
    }
}
