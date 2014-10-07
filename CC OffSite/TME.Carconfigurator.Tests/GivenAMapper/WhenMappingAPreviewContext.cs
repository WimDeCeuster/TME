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
    public class WhenMappingPreviewTimeFrames : TimeFramesTestBase
    {

        protected override void Arrange()
        {
            base.Arrange();

            context = new Context(brand, country, generation.ID, PublicationDataSubset.Preview);
        }

        protected override void Act()
        {
            mapper.Map(brand, country, generation.ID, generationFinder, context);
        }

        [Fact]
        public void ThenThereShouldBeOnly1TimeFrame()
        {
            var timeFrames = context.TimeFrames[language];

            timeFrames.Count.ShouldBeEquivalentTo(1);
        }

        [Fact]
        public void ThenTimeFrameShouldMatch()
        {
            var timeFrame = context.TimeFrames[language].Single();

            timeFrame.From.ShouldBeEquivalentTo(DateTime.MinValue);
            timeFrame.Until.ShouldBeEquivalentTo(DateTime.MaxValue);
            timeFrame.Cars.Count.ShouldBeEquivalentTo(context.ModelGenerations[language].Cars.Count);
        }
    }
}
