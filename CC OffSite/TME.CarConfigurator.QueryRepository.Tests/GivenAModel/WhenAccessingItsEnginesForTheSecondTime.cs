using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class WhenAccessingItsEnginesForTheSecondTime : TestBase
    {
        private Repository.Objects.Engine _engine1;
        private Repository.Objects.Engine _engine2;
        private IEnumerable<IEngine> _secondEngines;
        private IModel _model;
        private IEngineService _engineService;
        private IEnumerable<IEngine> _firstEngines;

        protected override void Arrange()
        {
            _engine1 = new EngineBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _engine2 = new EngineBuilder()
                .WithId(Guid.NewGuid())
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

            _engineService = A.Fake<IEngineService>();
            A.CallTo(() => _engineService.GetEngines(publication.ID, timeFrame.ID, context)).Returns(new[] { _engine1, _engine2 });

            var serviceFacade = new S3ServiceFacade()
                .WithConfigurationManager(new ConfigurationManagerBuilder().Build())
                .WithModelService(modelService)
                .WithPublicationService(publicationService)
                .WithEngineService(_engineService);

            var modelFactory = new ModelFactoryFacade()
                .WithServiceFacade(serviceFacade)
                .Create();

            _model = modelFactory.GetModels(context).Single();

            _firstEngines = _model.Engines;
        }

        protected override void Act()
        {
            _secondEngines = _model.Engines;
        }

        [Fact]
        public void ThenItShouldFetchTheEnginesFromTheService()
        {
            A.CallTo(() => _engineService.GetEngines(A<Guid>._, A<Guid>._, A<Context>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenTheListOfEnginesShouldBeCorrect()
        {
            _secondEngines.Should().BeSameAs(_firstEngines);
        }
    }
}
