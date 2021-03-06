﻿using System;
using FluentAssertions;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAnUpholstery
{
    public class WhenAccessingItsUpholsteryTypeForTheFirstTime : TestBase
    {
        IUpholstery _upholstery;
        IUpholsteryType _upholsteryType;
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
        }

        protected override void Act()
        {
            _upholsteryType = _upholstery.Type;
        }

        [Fact]
        public void ThenItShouldHaveTheType()
        {
            _upholsteryType.ID.Should().Be(_repoUpholsteryType.ID);
        }
    }
}
