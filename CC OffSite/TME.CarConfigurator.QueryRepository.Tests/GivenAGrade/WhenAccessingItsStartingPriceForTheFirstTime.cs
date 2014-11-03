using System;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Core;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAGrade
{
    public class WhenAccessingItsStartingPriceForTheFirstTime : TestBase
    {
        IGrade _grade;
        IPrice _price;
        Price _repoPrice;

        protected override void Arrange()
        {
            _repoPrice = new PriceBuilder()
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

            var gradeService = A.Fake<IGradeService>();
            A.CallTo(() => gradeService.GetGrades(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new [] { repoGrade });

            var gradeFactory = new GradeFactoryBuilder()
                .WithGradeService(gradeService)
                .Build();

            _grade = gradeFactory.GetGrades(publication, context).Single();
        }

        protected override void Act()
        {
            _price = _grade.StartingPrice;
        }

        [Fact]
        public void ThenThePriceShouldBeCorrect()
        {
            _price.PriceExVat.Should().Be(_repoPrice.ExcludingVat);
            _price.PriceInVat.Should().Be(_repoPrice.IncludingVat);
        }
    }
}
