using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Publisher.Enums;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using Xunit;
using FluentAssertions;

namespace TME.Carconfigurator.Tests.GivenAMapper
{
    public class WhenMappingAPreviewContext : TimeFramesTestBase
    {

        protected override void Arrange()
        {
            base.Arrange();

            _context = new Context(_brand, _country, _generation.ID, PublicationDataSubset.Preview);
        }

        protected override void Act()
        {
            _mapper.Map(_brand, _country, _generation.ID, _generationFinder, _context);
        }

        [Fact]
        public void ThenThereShouldBeOnly1TimeFrame()
        {
            var timeFrames = _context.TimeFrames[_language];

            timeFrames.Count.ShouldBeEquivalentTo(1);
        }

        [Fact]
        public void ThenTimeFrameShouldMatch()
        {
            var timeFrame = _context.TimeFrames[_language].Single();

            timeFrame.From.ShouldBeEquivalentTo(DateTime.MinValue);
            timeFrame.Until.ShouldBeEquivalentTo(DateTime.MaxValue);
            timeFrame.Cars.Count.ShouldBeEquivalentTo(_context.ModelGenerations[_language].Cars.Count);
        }
    }
}
