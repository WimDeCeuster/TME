using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.DI;
using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAModel
{
    public class WhenAccessingItsSteeringsForTheFirstTime : TestBase
    {
        private Repository.Objects.Steering _steering1;
        private Repository.Objects.Steering _steering2;
        private IEnumerable<ISteering> _steerings;
        private IModel _model;
        private ISteeringService _steeringService;

        protected override void Arrange()
        {
            _steering1 = new SteeringBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _steering2 = new SteeringBuilder()
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

            _steeringService = A.Fake<ISteeringService>();
            A.CallTo(() => _steeringService.GetSteerings(publication.ID, timeFrame.ID, context)).Returns(new [] {_steering1, _steering2});

            var serviceFacade = new S3ServiceFacade()
                .WithConfigurationManager(new ConfigurationManagerBuilder().Build())
                .WithModelService(modelService)
                .WithPublicationService(publicationService)
                .WithSteeringService(_steeringService);

            var modelFactory = new ModelFactoryFacade()
                .WithServiceFacade(serviceFacade)
                .Create();

            _model = modelFactory.GetModels(context).Single();
        }

        protected override void Act()
        {
            _steerings = _model.Steerings;
        }

        [Fact]
        public void ThenItShouldFetchTheSteeringsFromTheService()
        {
            A.CallTo(() => _steeringService.GetSteerings(A<Guid>._, A<Guid>._, A<Context>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenTheListOfSteeringsShouldBeCorrect()
        {
            _steerings.Should().HaveCount(2);

            _steerings.Should().Contain(steering => steering.ID == _steering1.ID);
            _steerings.Should().Contain(steering => steering.ID == _steering2.ID);
        }
    }
}
