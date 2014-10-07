using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Factories.Interfaces;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.QueryRepository.Interfaces;
using TME.CarConfigurator.QueryRepository.Tests.TestBuilders;
using TME.CarConfigurator.QueryRepository.Tests.TestBuilders.RepositoryObjects;
using TME.CarConfigurator.Repository.Objects.Enums;
using TME.CarConfigurator.Tests.Shared;
using Xunit;

namespace TME.CarConfigurator.QueryRepository.Tests.GivenModels
{
    public class WhenRequestingTheListOfActiveModels : TestBase
    {
        private const string ModelName1 = "ModelForContextLanguageWithAnActiveAndADeletedPublicationForTheSameGeneration";
        private const string ModelName2 = "ModelForContextLanguageWithAActivePublicationsOfWhichTheTimeFramesDoNotSpanTheCurrentDate";
        private const string ModelName3 = "ModelForContextLanguageWithAnArchivedPublication";

        private IEnumerable<Repository.Objects.Model> _modelsFromRespository;
        private IContext _context;
        private IModels _models;
        private IModelFactory _modelFactory;
        private IModelRepository _modelRepository;

        protected override void Arrange()
        {
            _context = ContextBuilder.InitializeFakeContext().Build();

            ArrangeModelsRepository();

            _modelFactory = ModelFactoryBuilder.Initialize().WithModelRepository(_modelRepository).Build();
        }

        private void ArrangeModelsRepository()
        {
            ArrangeModelsFromRepository();

            _modelRepository = ModelRepositoryBuilder.InitializeFakeRepository().Build();
            A.CallTo(() => _modelRepository.GetModels(null))
                .WhenArgumentsMatch(args => TestHelpers.Context.AreEqual((IContext) args[0], _context))
                .Returns(_modelsFromRespository);
        }

        private void ArrangeModelsFromRepository()
        {
            var models = new List<Repository.Objects.Model>
            {
                GetModelForContextLanguageWithActivePublicationsOfWhichTheTimeFramesDoNotSpanTheCurrentDate(),
                GetModelForContextLanguageWithAnActiveAndADeletedPublicationForTheSameGeneration(),
                GetModelForContextLanguageWithAnArchivedPublication()
            };

            _modelsFromRespository = models;
        }

        private static Repository.Objects.Model GetModelForContextLanguageWithAnActiveAndADeletedPublicationForTheSameGeneration()
        {
            var generation = GenerationBuilder.Initialize().Build();

            var activePublication = PublicationInfoBuilder.Initialize()
                .WithGeneration(generation)
                .WithDateRange(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(2))
                .WithState(PublicationState.Activated)
                .Build();

            var deletedPublication = PublicationInfoBuilder.Initialize()
                .WithGeneration(generation)
                .WithDateRange(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(2))
                .WithState(PublicationState.ToBeDeleted)
                .Build();

            var model = ModelBuilder.Initialize()
                .WithName(ModelName1)
                .AddPublication(activePublication)
                .AddPublication(deletedPublication)
                .Build();

            return model;
        }

        private static Repository.Objects.Model GetModelForContextLanguageWithActivePublicationsOfWhichTheTimeFramesDoNotSpanTheCurrentDate()
        {
            var generation = GenerationBuilder.Initialize().Build();

            var publicationInThePast = PublicationInfoBuilder.Initialize()
                .WithGeneration(generation)
                .WithDateRange(DateTime.Now.AddDays(-10), DateTime.Now.AddDays(-2))
                .WithState(PublicationState.Activated)
                .Build();

            var publicationInTheFuture = PublicationInfoBuilder.Initialize()
                .WithGeneration(generation)
                .WithDateRange(DateTime.Now.AddDays(3), DateTime.Now.AddDays(12))
                .WithState(PublicationState.Activated)
                .Build();

            var model = ModelBuilder.Initialize()
                .WithName(ModelName2)
                .AddPublication(publicationInThePast)
                .AddPublication(publicationInTheFuture)
                .Build();

            return model;
        }

        private static Repository.Objects.Model GetModelForContextLanguageWithAnArchivedPublication()
        {
            var generation = GenerationBuilder.Initialize().Build();

            var publication = PublicationInfoBuilder.Initialize()
                .WithGeneration(generation)
                .WithDateRange(DateTime.Now.AddDays(-10), DateTime.Now.AddDays(10))
                .WithState(PublicationState.Archived)
                .Build();

            var model = ModelBuilder.Initialize()
                .WithName(ModelName3)
                .AddPublication(publication)
                .Build();

            return model;
        }

        protected override void Act()
        {
            _models = Models.GetModels(_context, _modelFactory);
        }

        [Fact]
        public void ThenTheListShouldBeFetchedFromTheRepository()
        {
            A.CallTo(() => _modelRepository.GetModels(null))
                .WhenArgumentsMatch(args =>
                {
                    var contextInArgs = (IContext) args[0];
                    return TestHelpers.Context.AreEqual(contextInArgs, _context);
                })
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldOnlyContainTheModelThatHasAnActivePublicationThatIsAvailableToday()
        {
            _models.Should()
                .HaveCount(1, "because we don't want duplicate models")
                .And
                .OnlyContain(m => m.Name.Equals(ModelName1), "because this is the only model that has an active publication that is available today");
        }
    }
}