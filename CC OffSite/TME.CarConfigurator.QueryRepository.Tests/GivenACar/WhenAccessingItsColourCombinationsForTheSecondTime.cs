﻿using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenACar
{
    public class WhenAccessingItsColourCombinationsForTheSecondTime : TestBase
    {
        private ICar _car;
        private CarColourCombination _repoColourCombination;
        private IReadOnlyList<ICarColourCombination> _secondColourCombinations;
        private IReadOnlyList<ICarColourCombination> _firstColourCombinations;
        private IColourService _colourService;

        protected override void Arrange()
        {
            _repoColourCombination = new CarColourCombinationBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoCar = new CarBuilder()
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
            A.CallTo(() => carService.GetCars(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new[] {repoCar});

            _colourService = A.Fake<IColourService>();
            A.CallTo(() => _colourService.GetCarColourCombinations(publication.ID, context, repoCar.ID))
                .Returns(new [] { _repoColourCombination });

            var colourFactory = new ColourFactoryBuilder().WithColourService(_colourService).Build();
                

            var carFactory = new CarFactoryBuilder()
                .WithCarService(carService)
                .WithColourFactory(colourFactory)
                .Build();

            _car = carFactory.GetCars(publication, context).Single();

            _firstColourCombinations = _car.ColourCombinations;
        }

        protected override void Act()
        {
            _secondColourCombinations = _car.ColourCombinations;
        }

        [Fact]
        public void ThenItShouldNotRecalculateTheColourCombinations()
        {
            A.CallTo(() => _colourService.GetCarColourCombinations(A<Guid>._,A<Context>._,A<Guid>._)).MustHaveHappened(Repeated.Exactly.Once);
            _secondColourCombinations.Should().BeSameAs(_firstColourCombinations);
        }

        [Fact]
        public void ThenTheColourCombinationShouldBeCorrect()
        {
            _secondColourCombinations.Should().Contain(colourCombo => colourCombo.ID == _repoColourCombination.ID);
        }
    }
}