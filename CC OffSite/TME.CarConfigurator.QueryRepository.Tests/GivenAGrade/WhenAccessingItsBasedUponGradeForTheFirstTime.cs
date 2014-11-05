using System;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAGrade
{
    public class WhenAccessingItsBasedUponGradeForTheFirstTime : TestBase
    {
        IGrade _grade;
        IGradeInfo _basedUponGrade;
        Repository.Objects.Grade _repoBasedUponGrade;

        protected override void Arrange()
        {
            _repoBasedUponGrade = new GradeBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoGrade = new GradeBuilder()
                .WithId(Guid.NewGuid())
                .WithBasedUponGradeID(_repoBasedUponGrade.ID)
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
            A.CallTo(() => gradeService.GetGrades(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new [] { repoGrade, _repoBasedUponGrade });

            var gradeFactory = new GradeFactoryBuilder()
                .WithGradeService(gradeService)
                .Build();

            _grade = gradeFactory.GetGrades(publication, context).Single(grade => grade.ID == repoGrade.ID);
        }

        protected override void Act()
        {
            _basedUponGrade = _grade.BasedUpon;
        }

        [Fact]
        public void ThenTheBasedUponGradeShouldBeCorrect()
        {
            _basedUponGrade.ID.Should().Be(_repoBasedUponGrade.ID);
        }
    }
}
