using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenACar
{
    public class WhenAccessingItsEquipmentForTheSecondTime : TestBase
    {
        private ICarEquipment _secondEquipment;
        private CarEquipment _repositoryCarEquipment;
        private IEquipmentService _carEquipmentService;
        private ICar _cars;
        private ICarEquipment _firstEquipment;

        protected override void Arrange()
        {
            _repositoryCarEquipment = new CarEquipmentBuilder()
                .WithAccessories(
                    new CarAccessoryBuilder().WithId(Guid.NewGuid()).Build(),
                    new CarAccessoryBuilder().WithId(Guid.NewGuid()).Build())
                .WithOptions(
                    new CarOptionBuilder().WithId(Guid.NewGuid()).Build(),
                    new CarOptionBuilder().WithId(Guid.NewGuid()).Build())
                .Build();

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
            A.CallTo(() => carService.GetCars(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new List<Repository.Objects.Car> { repoCar });

            _carEquipmentService = A.Fake<IEquipmentService>(opt => opt.Strict());
            A.CallTo(() => _carEquipmentService.GetCarEquipment(repoCar.ID, publication.ID, context)).Returns(_repositoryCarEquipment);

            var carEquipmentFactory = new EquipmentFactoryBuilder()
                .WithEquipmentService(_carEquipmentService)
                .Build();

            var carFactory = new CarFactoryBuilder()
                .WithCarService(carService)
                .WithEquipmentFactory(carEquipmentFactory)
                .Build();

            _cars = carFactory.GetCars(publication, context).Single();

            _firstEquipment = _cars.Equipment;
        }

        protected override void Act()
        {
            _secondEquipment = _cars.Equipment;
        }

        [Fact]
        public void ThenItShouldNotFetchTheCarEquipmentFromTheServiceAgain()
        {
            A.CallTo(() => _carEquipmentService.GetCarEquipment(A<Guid>._, A<Guid>._, A<Context>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldHaveTheCorrectCarEquipment()
        {
            _secondEquipment.Should().BeSameAs(_firstEquipment);
        }
    }
}