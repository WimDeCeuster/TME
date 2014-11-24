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
    public class WhenAccessingItsUpholsteryForTheSecondTime : TestBase
    {
        ICarColourCombination _colourCombination;
        private IColourService _colourService;
        private ICarUpholstery _secondUpholstery;
        private CarUpholstery _repoCarUpholstery;
        private ICarUpholstery _firstUpholstery;

        protected override void Arrange()
        {
            var carID = Guid.NewGuid();

            _repoCarUpholstery = new CarUpholsteryBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoColourCombination = new CarColourCombinationBuilder()
                .WithUpholstery(_repoCarUpholstery)
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

            _firstUpholstery = _colourCombination.Upholstery;
        }

        protected override void Act()
        {
            _secondUpholstery = _colourCombination.Upholstery;
        }

        [Fact]
        public void ThenItShouldNotRecalculateTheUpholstery()
        {
            A.CallTo(() => _colourService.GetCarColourCombinations(A<Guid>._, A<Context>._, A<Guid>._)).MustHaveHappened(Repeated.Exactly.Once);
            _secondUpholstery.Should().BeSameAs(_firstUpholstery);
        }

        [Fact]
        public void ThenItShouldHaveTheCarUpholstery()
        {
            _secondUpholstery.ID.Should().Be(_repoCarUpholstery.ID);
        }
    }
}