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
    public class WhenAccessingItsNonExistentBasedUponGrade : TestBase
    {
        IGrade _grade;
        IGradeInfo _basedUponGrade;
        IGradeService _gradeService;
        
        protected override void Arrange()
        {
            var repoGrade = new GradeBuilder()
                .WithId(Guid.NewGuid())
                .WithBasedUponGradeID(Guid.Empty)
                .Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            _gradeService = A.Fake<IGradeService>();
            A.CallTo(() => _gradeService.GetGrades(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new [] { repoGrade });

            var gradeFactory = new GradeFactoryBuilder()
                .WithGradeService(_gradeService)
                .Build();

            _grade = gradeFactory.GetGrades(publication, context).Single();
        }

        protected override void Act()
        {
            _basedUponGrade = _grade.BasedUpon;
        }

        [Fact]
        public void ThenTheBasedUponGradeShouldBeCorrect()
        {
            _basedUponGrade.Should().BeNull();
        }
    }
}
