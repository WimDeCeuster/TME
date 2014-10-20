using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.DI;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAModel
{
    public class WhenAccessingTopLevelAndBasedUponGrades : TestBase
    {
        private Repository.Objects.Grade _grade1;
        private Repository.Objects.Grade _grade2;
        private IEnumerable<IGrade> _grades;
        private IGrade _basedUponGrade;
        private IModel _model;
        private IGradeService _gradeService;

        protected override void Arrange()
        {
            _grade1 = new GradeBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _grade2 = new GradeBuilder()
                .WithId(Guid.NewGuid())
                .WithBasedUponGradeID(_grade1.ID)
                .Build();

            var generation = new GenerationBuilder().Build();

            var timeFrame = new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .WithGeneration(generation)
                .AddTimeFrame(timeFrame)
                .Build();

            var publicationInfo = new PublicationInfo(publication) { State = Repository.Objects.Enums.PublicationState.Activated };

            var repoModel = new ModelBuilder().AddPublication(publicationInfo).Build();

            var context = ContextBuilder.Initialize().Build();

            var publicationService = A.Fake<IPublicationService>();
            A.CallTo(() => publicationService.GetPublication(publication.ID, context)).Returns(publication);

            var modelService = A.Fake<IModelService>();
            A.CallTo(() => modelService.GetModels(A<Context>._)).Returns(new[] { repoModel });

            _gradeService = A.Fake<IGradeService>();
            A.CallTo(() => _gradeService.GetGrades(publication.ID, timeFrame.ID, context)).Returns(new[] { _grade1, _grade2 });

            var serviceFacade = new S3ServiceFacade()
                .WithConfigurationManager(new ConfigurationManagerBuilder().Build())
                .WithModelService(modelService)
                .WithPublicationService(publicationService)
                .WithGradeService(_gradeService);

            var modelFactory = new ModelFactoryFacade()
                .WithServiceFacade(serviceFacade)
                .Create();

            _model = modelFactory.GetModels(context).Single();
        }

        protected override void Act()
        {
            _grades = _model.Grades;
            _basedUponGrade = _grades.Last().BasedUpon;
        }

        [Fact]
        public void ThenItShouldFetchTheGradesFromTheService()
        {
            A.CallTo(() => _gradeService.GetGrades(A<Guid>._, A<Guid>._, A<Context>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenTheBasedUponGradeShouldBeTheSame()
        {
            _basedUponGrade.Should().Be(_grades.First());
        }
    }
}
