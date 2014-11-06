using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAColourCombination
{
    public class WhenAccessingItsExteriorColourForTheFirstTime : TestBase
    {
        IColourCombination _colourCombination;
        IExteriorColour _exteriorColour;
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

            var colourService = A.Fake<IColourService>();
            A.CallTo(() => colourService.GetColourCombinations(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new [] { repoColourCombination });

            var colourFactory = new ColourFactoryBuilder()
                .WithColourService(colourService)
                .Build();

            _colourCombination = colourFactory.GetColourCombinations(publication, context).Single();
        }

        protected override void Act()
        {
            _exteriorColour = _colourCombination.ExteriorColour;
        }

        [Fact]
        public void ThenItShouldHaveTheExteriorColour()
        {
            _exteriorColour.ID.Should().Be(_repoExteriorColour.ID);
        }
    }
}
