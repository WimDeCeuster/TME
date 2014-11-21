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
    public class WhenAccessingItsEquipmentForTheFirstTime : TestBase
    {
        ICarPack _carPack;
        ICarPackEquipment _equipment;
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
            A.CallTo(() => packService.GetCarPacks(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new [] { repoCarPack });

            var equipmentFactory = new EquipmentFactoryBuilder().Build();

            var packFactory = new PackFactoryBuilder()
                .WithPackService(packService)
                .WithEquipmentFactory(equipmentFactory)
                .Build();

            _carPack = packFactory.GetCarPacks(publication, context, Guid.Empty).Single();
        }

        protected override void Act()
        {
            _equipment = _carPack.Equipment;
        }

        [Fact]
        public void ThenTheCarPackEquipmentShouldBeCorrect()
        {
            _equipment.Accessories.Count.Should().Be(_repoCarPackEquipment.Accessories.Count);
        }
    }
}
