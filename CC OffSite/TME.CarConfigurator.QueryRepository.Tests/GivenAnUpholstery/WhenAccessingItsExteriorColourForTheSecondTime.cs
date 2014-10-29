using FakeItEasy;
using FluentAssertions;
using System;
using System.Linq;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAColourCombination
{
    public class WhenAccessingItsExteriorColourForTheSecondTime : TestBase
    {
        IColourCombination _colourCombination;
        IExteriorColour _firstExteriorColour;
        IExteriorColour _secondExteriorColour;
        Repository.Objects.Colours.ExteriorColour _repoExteriorColour;

        protected override void Arrange()
        {
            _repoExteriorColour = new ExteriorColourBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoColourCombination = new ColourCombinationBuilder()
                .WithExteriorColour(_repoExteriorColour)
                .Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            var colourCombinationService = A.Fake<IColourService>();
            A.CallTo(() => colourCombinationService.GetColourCombinations(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new [] { repoColourCombination });

            var colourFactory = new ColourFactoryBuilder()
                .WithColourService(colourCombinationService)
                .Build();

            _colourCombination = colourFactory.GetColourCombinations(publication, context).Single();

            _firstExteriorColour = _colourCombination.ExteriorColour; ;
        }

        protected override void Act()
        {
            _secondExteriorColour = _colourCombination.ExteriorColour;
        }

        [Fact]
        public void ThenItShouldNotRecalculateTheType()
        {
            _secondExteriorColour.Should().Be(_firstExteriorColour);
        }

        [Fact]
        public void ThenItShouldHaveTheType()
        {
            _secondExteriorColour.ID.Should().Be(_repoExteriorColour.ID);
        }
    }
}
