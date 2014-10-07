using System;
using System.Linq;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Publisher.Enums;
using Xunit;
using FluentAssertions;

namespace TME.Carconfigurator.Tests.GivenAMapper
{
    public class WhenMappingALiveContext : TimeFramesTestBase
    {

        protected override void Arrange()
        {
            base.Arrange();

            _context = new Context(_brand, _country, _generation.ID, PublicationDataSubset.Live);
        }

        protected override void Act()
        {
            _mapper.Map(_brand, _country, _generation.ID, _generationFinder, _context);
        }

        [Fact]
        public void ThenThereShouldBeSeveralTimeFrames()
        {
            var timeFrames = _context.TimeFrames[_language];

            if (_context.ContextData[_language].Cars.Count == _minimumCarCount)
                timeFrames.Count.ShouldBeEquivalentTo(9);
            else
                timeFrames.Count.ShouldBeEquivalentTo(11);
        }

        [Fact]
        public void ThenLastTimeFrameShouldMatch()
        {
            var lastTimeFrame = _context.TimeFrames[_language].Last();

            var latestCar = _context.ModelGenerations[_language].Cars.OrderBy(car => car.LineOffToDate).Last();

            lastTimeFrame.From.ShouldBeEquivalentTo(new DateTime(2014, 11, 1));
            lastTimeFrame.Until.ShouldBeEquivalentTo(new DateTime(2014, 12, 1));
            lastTimeFrame.Cars.Single().ID.ShouldBeEquivalentTo(latestCar.ID);
        }

        [Fact]
        public void ThenLast2TimeFramesShouldMatch()
        {
            var lastTimeFrame = _context.TimeFrames[_language].Reverse().Skip(1).First();

            var latestCars = _context.ModelGenerations[_language].Cars.OrderBy(car => car.LineOffToDate)
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
