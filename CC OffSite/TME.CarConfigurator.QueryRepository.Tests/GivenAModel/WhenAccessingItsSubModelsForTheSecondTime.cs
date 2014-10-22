using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
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
    public class WhenAccessingItsSubModelsForTheSecondTime : TestBase
    {
        private IModel _model;
        private Repository.Objects.SubModel _subModel1;
        private Repository.Objects.SubModel _subModel2;
        private ISubModelService _subModelService;
        private IEnumerable<ISubModel> _secondSubModels;
        private IEnumerable<ISubModel> _firstSubModels;

        protected override void Arrange()
        {
            _subModel1 = new SubModelBuilder()
                .WithID(Guid.NewGuid())
                .Build();

            _subModel2 = new SubModelBuilder()
                .WithID(Guid.NewGuid())
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

            _subModelService = A.Fake<ISubModelService>();
            A.CallTo(() => _subModelService.GetSubModels(publication.ID, timeFrame.ID, context)).Returns(new[] { _subModel1, _subModel2 });

            var serviceFacade = new S3ServiceFacade()
                .WithConfigurationManager(new ConfigurationManagerBuilder().Build())
                .WithModelService(modelService)
                .WithPublicationService(publicationService)
                .WithSubModelService(_subModelService);

            var modelFactory = new ModelFactoryFacade()
                .WithServiceFacade(serviceFacade)
                .Create();

            _model = modelFactory.GetModels(context).Single();

            _firstSubModels = _model.SubModels;
        }

        protected override void Act()
        {
            _secondSubModels = _model.SubModels;
        }

        [Fact]
        public void ThenItShouldNotFetchTheSubModelsFromTheServiceAgain()
        {
            A.CallTo(() => _subModelService.GetSubModels(A<Guid>._, A<Guid>._, A<Context>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenTheListOfSteeringsShouldBeCorrect()
        {
            _secondSubModels.Should().BeSameAs(_firstSubModels);
        }
    }
}