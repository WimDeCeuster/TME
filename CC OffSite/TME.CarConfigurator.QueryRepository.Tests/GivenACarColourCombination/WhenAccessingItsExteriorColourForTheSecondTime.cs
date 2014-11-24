using System;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenACarColourCombination
{
    public class WhenAccessingItsExteriorColourForTheSecondTime : TestBase
    {
        ICarColourCombination _colourCombination;
        ICarExteriorColour _secondExteriorColour;
        CarExteriorColour _repoExteriorColour;
        ICarExteriorColour _firstExteriorColour;
        private IColourService _colourService;

        protected override void Arrange()
        {
            var carID = Guid.NewGuid();

            _repoExteriorColour = new CarExteriorColourBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoColourCombination = new CarColourCombinationBuilder()
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
            
            _colourService = A.Fake<IColourService>();
            A.CallTo(() => _colourService.GetCarColourCombinations(A<Guid>._, A<Context>._, A<Guid>._)).Returns(new[] { repoColourCombination });

            var colourFactory = new ColourFactoryBuilder()
                .WithColourService(_colourService)
                .Build();

            _colourCombination = colourFactory.GetCarColourCombinations(publication, context, carID).Single();

            _firstExteriorColour = _colourCombination.ExteriorColour;
        }

        protected override void Act()
        {
            _secondExteriorColour = _colourCombination.ExteriorColour;
        }

        [Fact]
        public void ThenItShouldNotRecalculateTheExteriorColour()
        {
            A.CallTo(() => _colourService.GetCarColourCombinations(A<Guid>._, A<Context>._, A<Guid>._)).MustHaveHappened(Repeated.Exactly.Once);
            _secondExteriorColour.Should().BeSameAs(_firstExteriorColour);
        }

        [Fact]
        public void ThenItShouldHaveTheCarExteriorColour()
        {
            _secondExteriorColour.ID.Should().Be(_repoExteriorColour.ID);
        }
    }
}