using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenACar
{
    public class WhenAccessingItsPartsForTheSecondTime : TestBase
    {
        private ICar _car;
        private IReadOnlyList<ICarPart> _secondParts;
        private ICarPartService _carPartService;
        private Repository.Objects.CarPart _carPart1;
        private Repository.Objects.CarPart _carPart2;
        private IReadOnlyList<ICarPart> _firstParts;
        const string CODE1 = "the first code";
        const string CODE2 = "the second code";
        const string NAME1 = "the first name";
        const string NAME2 = "the second name";

        protected override void Arrange()
        {
            _carPart1 = new CarPartBuilder().WithCode(CODE1).WithName(NAME1).Build();
            _carPart2 = new CarPartBuilder().WithCode(CODE2).WithName(NAME2).Build();

            var car1 = new CarBuilder().WithId(Guid.NewGuid()).Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder().WithID(Guid.NewGuid()).WithDateRange(DateTime.MinValue, DateTime.MaxValue).Build();

            var publication = new PublicationBuilder().WithID(Guid.NewGuid()).WithTimeFrames(publicationTimeFrame).Build();

            var context = new ContextBuilder().Build();

            _carPartService = A.Fake<ICarPartService>();
            A.CallTo(() => _carPartService.GetCarParts(publication.ID, car1.ID, context)).Returns(new[] { _carPart1, _carPart2 });

            var carPartFactory = new CarPartFactoryBuilder().WithCarPartService(_carPartService).Build();

            var carService = A.Fake<ICarService>();
            A.CallTo(() => carService.GetCars(publication.ID, publicationTimeFrame.ID, context)).Returns(new[] { car1 });

            var carFactory = new CarFactoryBuilder().WithCarService(carService).WithCarPartFactory(carPartFactory).Build();

            _car = carFactory.GetCars(publication, context).Single();

            _firstParts = _car.Parts;
        }

        protected override void Act()
        {
            _secondParts = _car.Parts;
        }

        [Fact]
        public void ThenThePartsShouldNotBeRecalculated()
        {
            _secondParts.Should().BeSameAs(_firstParts);
        }

        [Fact]
        public void ThenTheCarPartsShouldBeCorrect()
        {
            _secondParts.Should().HaveCount(2);
            _secondParts.Should().Contain(p => p.Code == _carPart1.Code);
            _secondParts.Should().Contain(p => p.Name == _carPart1.Name);
            _secondParts.Should().Contain(p => p.Code == _carPart2.Code);
            _secondParts.Should().Contain(p => p.Name == _carPart2.Name);
        }
    }
}