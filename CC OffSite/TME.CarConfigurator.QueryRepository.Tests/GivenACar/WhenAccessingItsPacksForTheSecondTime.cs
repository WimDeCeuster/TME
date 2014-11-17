using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Packs;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenACar
{
    public class WhenAccessingItsPacksForTheSecondTime : TestBase
    {
        private Repository.Objects.Packs.CarPack _pack1;
        private Repository.Objects.Packs.CarPack _pack2;
        private ICar _car;
        private IEnumerable<ICarPack> _firstPacks;
        private IEnumerable<ICarPack> _secondPacks;
        private IPackService _packService;

        protected override void Arrange()
        {
            _pack1 = new CarPackBuilder().Build();
            _pack2 = new CarPackBuilder().Build();

            var repoCar = new CarBuilder()
                .WithId(Guid.NewGuid())
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

            _packService = A.Fake<IPackService>();
            A.CallTo(() => _packService.GetCarPacks(A<Guid>._, repoCar.ID, A<Context>._)).Returns(new [] { _pack1, _pack2 });

            var packFactory = new PackFactoryBuilder()
                .WithPackService(_packService)
                .Build();

            var carFactory = new CarFactoryBuilder()
                .WithCarService(carService)
                .WithPackFactory(packFactory)
                .Build();

            _car = carFactory.GetCars(publication, context).Single();

            _firstPacks = _car.Packs;
        }

        protected override void Act()
        {
            _secondPacks = _car.Packs;
        }

        [Fact]
        public void ThenItShouldNotFetchThePacksFromTheServiceAgain()
        {
            A.CallTo(() => _packService.GetCarPacks(A<Guid>._, A<Guid>._, A<Context>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldReferenceTheSameListOfPacks()
        {
            _secondPacks.Should().BeSameAs(_firstPacks);
        }
    }
}