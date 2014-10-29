using System;
using System.Collections.Generic;
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

namespace TME.CarConfigurator.Query.Tests.GivenASubModel
{
    public class WhenAccessingItsGradesForTheSecondTime : TestBase
    {
        private ISubModel _subModel;
        private IEnumerable<IGrade> _secondGrades;
        private Repository.Objects.Grade _grade2;
        private Repository.Objects.Grade _grade1;
        private IGradeService _gradeService;
        private IEnumerable<IGrade> _firstGrades;

        protected override void Arrange()
        {
            _grade1 = new GradeBuilder().WithId(Guid.NewGuid()).Build();
            _grade2 = new GradeBuilder().WithId(Guid.NewGuid()).Build();

            var repositorySubModel = new SubModelBuilder()
                .WithID(Guid.NewGuid())
                .WithGrades(_grade1, _grade2)
                .Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            var subModelService = A.Fake<ISubModelService>();
            A.CallTo(() => subModelService.GetSubModels(A<Guid>._, A<Guid>._, A<Context>._))
                .Returns(new List<Repository.Objects.SubModel> { repositorySubModel });

            _gradeService = A.Fake<IGradeService>();
            A.CallTo(() => _gradeService.GetGrades(publication.ID, publicationTimeFrame.ID, context))
                .Returns(new List<Repository.Objects.Grade>() { _grade1, _grade2 });

            var gradeFactory = new GradeFactoryBuilder()
                .WithGradeService(_gradeService)
                .Build();

            var subModelFactory = new SubModelFactoryBuilder()
                .WithSubModelService(subModelService)
                .WithGradeFactory(gradeFactory)
                .Build();

            _subModel = subModelFactory.GetSubModels(publication, context).Single();

            _firstGrades = _subModel.Grades;
        }

        protected override void Act()
        {
            _secondGrades = _subModel.Grades;
        }

        [Fact(Skip = "Is this test Still Relevant? SubmodelGrades Are already Picked Up When GetSubModels is Called.")]
        public void ThenItShouldNotFetchTheGradesFromTheServiceAgain()
        {
            A.CallTo(() => _gradeService.GetGrades(A<Guid>._, A<Guid>._, A<Context>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldReferToTheSameGradesAsTheFirstTime()
        {
            _secondGrades.Should().BeSameAs(_firstGrades);
        } 
    }
}