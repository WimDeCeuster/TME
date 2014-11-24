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
    public class WhenAccessingItsExteriorColourForTheFirstTime : TestBase
    {
        ICarColourCombination _colourCombination;
        ICarExteriorColour _exteriorColour;
        CarExteriorColour _repoExteriorColour;

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

            var colourService = A.Fake<IColourService>();
            A.CallTo(() => colourService.GetCarColourCombinations(A<Guid>._, A<Context>._, A<Guid>._)).Returns(new[] { repoColourCombination });

            var colourFactory = new ColourFactoryBuilder()
                .WithColourService(colourService)
                .Build();

            _colourCombination = colourFactory.GetCarColourCombinations(publication, context, carID).Single();
        }

        protected override void Act()
        {
            _exteriorColour = _colourCombination.ExteriorColour;
        }

        [Fact]
        public void ThenItShouldHaveTheCarExteriorColour()
        {
            _exteriorColour.ID.Should().Be(_repoExteriorColour.ID);
        }
    }
}