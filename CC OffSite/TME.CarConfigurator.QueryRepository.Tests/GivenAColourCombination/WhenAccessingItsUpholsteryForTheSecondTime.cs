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
    public class WhenAccessingItsUpholsteryForTheSecondTime : TestBase
    {
        IColourCombination _colourCombination;
        IUpholstery _firstUpholstery;
        IUpholstery _secondUpholstery;
        Repository.Objects.Colours.Upholstery _repoUpholstery;

        protected override void Arrange()
        {
            _repoUpholstery = new UpholsteryBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoColourCombination = new ColourCombinationBuilder()
                .WithUpholstery(_repoUpholstery)
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

            _firstUpholstery = _colourCombination.Upholstery; ;
        }

        protected override void Act()
        {
            _secondUpholstery = _colourCombination.Upholstery;
        }

        [Fact]
        public void ThenItShouldNotRecalculateTheUpholstery()
        {
            _secondUpholstery.Should().Be(_firstUpholstery);
        }

        [Fact]
        public void ThenItShouldHaveTheUpholstery()
        {
            _secondUpholstery.ID.Should().Be(_repoUpholstery.ID);
        }
    }
}
