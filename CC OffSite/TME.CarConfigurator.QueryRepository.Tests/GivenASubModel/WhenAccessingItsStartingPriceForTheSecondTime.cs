using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenASubModel
{
    public class WhenAccessingItsStartingPriceForTheSecondTime : TestBase
    {
        private ISubModel _subModel;
        private IPrice _secondTimeStartingPrice;
        private Repository.Objects.Core.Price _repoPrice;
        private IPrice _firstTimeStartingPrice;

        protected override void Arrange()
        {
            _repoPrice = new PriceBuilder()
                .WithPriceExVat(24)
                .WithPriceInVat(36)
                .Build();

            var repositorySubModel = new SubModelBuilder()
                .WithID(Guid.NewGuid())
                .WithStartingPrice(_repoPrice)
                .Build();

            var publicationTimeFrame =
                new PublicationTimeFrameBuilder()
                .WithID(Guid.NewGuid())
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            var subModelService = A.Fake<ISubModelService>();
            A.CallTo(() => subModelService.GetSubModels(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new List<Repository.Objects.SubModel>() { repositorySubModel });

            var subModelFactory = new SubModelFactoryBuilder()
                .WithSubModelService(subModelService)
                .Build();

            _subModel = subModelFactory.GetSubModels(publication, context).Single();

            _firstTimeStartingPrice = _subModel.StartingPrice;
        }

        protected override void Act()
        {
            _secondTimeStartingPrice = _subModel.StartingPrice;
        }

        [Fact]
        public void ThenItShouldNotRecalculateThePrice()
        {
            _secondTimeStartingPrice.Should().BeSameAs(_firstTimeStartingPrice);
        }

        [Fact]
        public void ThenTheStartingPriceShouldBeCorrect()
        {
            _secondTimeStartingPrice.PriceExVat.Should().Be(_repoPrice.ExcludingVat);
            _secondTimeStartingPrice.PriceInVat.Should().Be(_repoPrice.IncludingVat);
        }
    }
}