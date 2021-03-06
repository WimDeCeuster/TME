﻿using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.DI;
using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAModel
{
    public class WhenAccessingItsColourCombinationsForTheFirstTime : TestBase
    {
        private Repository.Objects.Colours.ColourCombination _colourCombination1;
        private Repository.Objects.Colours.ColourCombination _colourCombination2;
        private IEnumerable<IColourCombination> _colourCombinations;
        private IModel _model;
        private IColourService _colourCombinationService;

        protected override void Arrange()
        {
            _colourCombination1 = new ColourCombinationBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _colourCombination2 = new ColourCombinationBuilder()
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

            _colourCombinationService = A.Fake<IColourService>();
            A.CallTo(() => _colourCombinationService.GetColourCombinations(publication.ID, timeFrame.ID, context)).Returns(new [] {_colourCombination1, _colourCombination2});

            var serviceFacade = new S3ServiceFacade()
                .WithConfigurationManager(new ConfigurationManagerBuilder().Build())
                .WithModelService(modelService)
                .WithPublicationService(publicationService)
                .WithColourService(_colourCombinationService);

            var modelFactory = new ModelFactoryFacade()
                .WithServiceFacade(serviceFacade)
                .Create();

            _model = modelFactory.GetModels(context).Single();
        }

        protected override void Act()
        {
            _colourCombinations = _model.ColourCombinations;
        }

        [Fact]
        public void ThenItShouldFetchTheColourCombinationsFromTheService()
        {
            A.CallTo(() => _colourCombinationService.GetColourCombinations(A<Guid>._, A<Guid>._, A<Context>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenTheListOfColourCombinationsShouldBeCorrect()
        {
            _colourCombinations.Should().HaveCount(2);

            _colourCombinations.Should().Contain(colourCombination => colourCombination.ID == _colourCombination1.ID);
            _colourCombinations.Should().Contain(colourCombination => colourCombination.ID == _colourCombination2.ID);
        }
    }
}
