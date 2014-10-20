using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Core;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.GradeConfigurator.Query.Tests.GivenAGrade
{
    public class WhenAccessingItsStartingPriceForTheSecondTime : TestBase
    {
        IGrade _car;
        IPrice _firstPrice;
        IPrice _secondPrice;
        Price _repoPrice;

        protected override void Arrange()
        {
            _repoPrice = new CarConfigurator.Tests.Shared.TestBuilders.PriceBuilder()
                .WithPriceExVat(5)
                .WithPriceExVat(10)
                .Build();

            var repoGrade = new GradeBuilder()
                .WithStartingPrice(_repoPrice)
                .Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            var carService = A.Fake<IGradeService>();
            A.CallTo(() => carService.GetGrades(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new[] { repoGrade });

            var carFactory = new GradeFactoryBuilder()
                .WithGradeService(carService)
                .Build();

            _car = carFactory.GetGrades(publication, context).Single();

            _firstPrice = _car.StartingPrice;
        }

        protected override void Act()
        {
            _secondPrice = _car.StartingPrice;
        }

        [Fact]
        public void ThenItShouldNotRecalculateThePrice()
        {
            _secondPrice.Should().Be(_firstPrice);
        }

        [Fact]
        public void ThenThePriceShouldBeCorrect()
        {
            _secondPrice.PriceExVat.Should().Be(_repoPrice.ExcludingVat);
            _secondPrice.PriceInVat.Should().Be(_repoPrice.IncludingVat);
        }


    }
}
