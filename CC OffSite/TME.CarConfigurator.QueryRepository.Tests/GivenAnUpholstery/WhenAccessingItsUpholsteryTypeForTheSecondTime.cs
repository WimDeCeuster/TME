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

namespace TME.CarConfigurator.Query.Tests.GivenAUpholstery
{
    public class WhenAccessingItsUpholsteryTypeForTheSecondTime : TestBase
    {
        IUpholstery _upholstery;
        IUpholsteryType _firstUpholsteryType;
        IUpholsteryType _secondUpholsteryType;
        Repository.Objects.Colours.UpholsteryType _repoUpholsteryType;

        protected override void Arrange()
        {
            _repoUpholsteryType = new UpholsteryTypeBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoUpholstery = new UpholsteryBuilder()
                .WithUpholsteryType(_repoUpholsteryType)
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

            _upholstery = colourFactory.GetUpholstery(repoUpholstery, publication, context);

            _firstUpholsteryType = _upholstery.Type;
        }

        protected override void Act()
        {
            _secondUpholsteryType = _upholstery.Type;
        }

        [Fact]
        public void ThenItShouldNotRecalculateTheType()
        {
            _secondUpholsteryType.Should().Be(_firstUpholsteryType);
        }

        [Fact]
        public void ThenItShouldHaveTheType()
        {
            _secondUpholsteryType.ID.Should().Be(_repoUpholsteryType.ID);
        }
    }
}
