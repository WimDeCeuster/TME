using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using System;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Query.Tests.GivenACar
{
    public class WhenAccessingItsStartingPriceForTheFirstTime : TestBase
    {
        ICar _car;
        IPrice _price;
        Repository.Objects.Core.Price _repoPrice;

        protected override void Arrange()
        {
            _repoPrice = new CarConfigurator.Tests.Shared.TestBuilders.PriceBuilder()
                .WithPriceExVat(5)
                .WithPriceExVat(10)
                .Build();

            var repoCar = new CarBuilder()
                .WithStartingPrice(_repoPrice)
                .Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            var carService = A.Fake<ICarService>();
            A.CallTo(() => carService.GetCars(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new [] { repoCar });

            var carFactory = new CarFactoryBuilder()
                .WithCarService(carService)
                .Build();

            _car = carFactory.GetCars(publication, context).Single();
        }

        protected override void Act()
        {
            _price = _car.StartingPrice;
        }

        [Fact]
        public void ThenThePriceShouldBeCorrect()
        {
            _price.PriceExVat.Should().Be(_repoPrice.ExcludingVat);
            _price.PriceInVat.Should().Be(_repoPrice.IncludingVat);
        }
    }
}
