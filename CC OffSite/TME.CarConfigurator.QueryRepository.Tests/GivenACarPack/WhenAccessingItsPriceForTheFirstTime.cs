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

namespace TME.CarConfigurator.Query.Tests.GivenACarPack
{
    public class WhenAccessingItsPriceForTheFirstTime : TestBase
    {
        ICarPack _carPack;
        IPrice _price;
        Price _repoPrice;

        protected override void Arrange()
        {
            _repoPrice = new PriceBuilder()
                .WithPriceExVat(5)
                .WithPriceExVat(10)
                .Build();

            var repoCarPack = new CarPackBuilder()
                .WithPrice(_repoPrice)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .Build();

            var context = new ContextBuilder().Build();

            var packService = A.Fake<IPackService>();
            A.CallTo(() => packService.GetCarPacks(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new [] { repoCarPack });

            var packFactory = new PackFactoryBuilder()
                .WithPackService(packService)
                .Build();

            _carPack = packFactory.GetCarPacks(publication, context, Guid.Empty).Single();
        }

        protected override void Act()
        {
            _price = _carPack.Price;
        }

        [Fact]
        public void ThenThePriceShouldBeCorrect()
        {
            _price.PriceExVat.Should().Be(_repoPrice.ExcludingVat);
            _price.PriceInVat.Should().Be(_repoPrice.IncludingVat);
        }
    }
}
