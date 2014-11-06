using System;
using FluentAssertions;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAnExteriorColour
{
    public class WhenAccessingItsExteriorColourTypeForTheFirstTime : TestBase
    {
        IExteriorColour _exteriorColour;
        IExteriorColourType _exteriorColourType;
        Repository.Objects.Colours.ExteriorColourType _repoExteriorColourType;

        protected override void Arrange()
        {
            _repoExteriorColourType = new ExteriorColourTypeBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoExteriorColour = new ExteriorColourBuilder()
                .WithExteriorColourType(_repoExteriorColourType)
                .Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            var colourFactory = new ColourFactoryBuilder()
                .Build();

            _exteriorColour = colourFactory.GetExteriorColour(repoExteriorColour, publication, context);
        }

        protected override void Act()
        {
            _exteriorColourType = _exteriorColour.Type;
        }

        [Fact]
        public void ThenItShouldHaveTheType()
        {
            _exteriorColourType.ID.Should().Be(_repoExteriorColourType.ID);
        }
    }
}
