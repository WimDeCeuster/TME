using System;
using System.Linq;
using TME.CarConfigurator.Publisher;
using Xunit;
using FluentAssertions;
using TME.CarConfigurator.Publisher.Enums;

namespace TME.Carconfigurator.Tests.GivenAMapper
{
    public class WhenMappingLiveTimeFrames : TimeFramesTestBase
    {

        protected override void Arrange()
        {
            base.Arrange();

            context = new Context(brand, country, generation.ID, PublicationDataSubset.Live);
        }

        protected override void Act()
        {
            mapper.Map(brand, country, generation.ID, generationFinder, context);
        }

        [Fact]
        public void ThenThereShouldBeSeveralTimeFrames()
        {
            var timeFrames = context.TimeFrames[language];

            if (context.ContextData[language].Cars.Count == minimumCarCount)
                timeFrames.Count.ShouldBeEquivalentTo(9);
            else
                timeFrames.Count.ShouldBeEquivalentTo(11);
        }

        [Fact]
        public void ThenLastTimeFrameShouldMatch()
        {
            var lastTimeFrame = context.TimeFrames[language].Last();

            var latestCar = context.ModelGenerations[language].Cars.OrderBy(car => car.LineOffToDate).Last();

            lastTimeFrame.From.ShouldBeEquivalentTo(new DateTime(2014, 11, 1));
            lastTimeFrame.Until.ShouldBeEquivalentTo(new DateTime(2014, 12, 1));
            lastTimeFrame.Cars.Single().ID.ShouldBeEquivalentTo(latestCar.ID);
        }

        [Fact]
        public void ThenLast2TimeFramesShouldMatch()
        {
            var lastTimeFrame = context.TimeFrames[language].Reverse().Skip(1).First();

            var latestCars = context.ModelGenerations[language].Cars.OrderBy(car => car.LineOffToDate)
                                                                      .Reverse()
                                                                      .Take(2)
                                                                      .Reverse()
                                                                      .ToArray();

            lastTimeFrame.From.ShouldBeEquivalentTo(new DateTime(2014, 10, 1));
            lastTimeFrame.Until.ShouldBeEquivalentTo(new DateTime(2014, 11, 1));
            lastTimeFrame.Cars.Count.ShouldBeEquivalentTo(2);
            lastTimeFrame.Cars.First().ID.ShouldBeEquivalentTo(latestCars.First().ID);
            lastTimeFrame.Cars.Last().ID.ShouldBeEquivalentTo(latestCars.Last().ID);
        }       
    }
}
