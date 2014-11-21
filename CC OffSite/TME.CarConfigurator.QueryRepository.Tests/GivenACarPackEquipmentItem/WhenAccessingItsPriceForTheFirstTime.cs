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

namespace TME.CarConfigurator.Query.Tests.GivenACarPackEquipmentItem
{
    public class WhenAccessingItsPriceForTheFirstTime : TestBase
    {
        ICarPackEquipmentItem _carPackEquipmentItem;
        IPrice _price;
        Price _repoPrice;

        protected override void Arrange()
        {
            _repoPrice = new PriceBuilder()
                .WithPriceExVat(5)
                .WithPriceExVat(10)
                .Build();

            var repoCarPackEquipmentItem = new CarPackOptionBuilder()
                .WithPrice(_repoPrice)
                .Build();

            var repoCarPackEquipment = new CarPackEquipmentBuilder()
                .WithOptions(repoCarPackEquipmentItem)
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
            _carPackEquipmentItem = carPack.Equipment.Options.Single();
        }

        protected override void Act()
        {
            _price = _carPackEquipmentItem.Price;
        }

        [Fact]
        public void ThenThePriceShouldBeCorrect()
        {
            _price.PriceExVat.Should().Be(_repoPrice.ExcludingVat);
            _price.PriceInVat.Should().Be(_repoPrice.IncludingVat);
        }
    }
}
