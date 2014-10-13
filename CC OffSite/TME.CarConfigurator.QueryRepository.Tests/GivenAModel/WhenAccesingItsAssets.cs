using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.DI;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Enums;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders.RepositoryObjects;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAModel
{
    public class WhenAccesingItsAssets : TestBase
    {
        private IModel _model;
        private IEnumerable<IAsset> _assets;
        private Repository.Objects.Assets.Asset _asset1;
        private Repository.Objects.Assets.Asset _asset2;

        protected override void Arrange()
        {
            _asset1 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();
            _asset2 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var publicationId = Guid.NewGuid();
            var generation = new GenerationBuilder()
                .AddAsset(_asset1)
                .AddAsset(_asset2)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(publicationId)
                .WithGeneration(generation)
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publicationInfo = new PublicationInfo(publication) { State = PublicationState.Activated };

            var repoModel = new ModelBuilder().AddPublication(publicationInfo).Build();

            var publicationFactory = A.Fake<IPublicationFactory>();
            A.CallTo(() => publicationFactory.GetPublication(repoModel, A<Context>._)).Returns(publication);

            var modelService = A.Fake<IModelService>();
            A.CallTo(() => modelService.GetModels(A<Context>._)).Returns(new List<Repository.Objects.Model> { repoModel });

            var configurationManager = new ConfigurationManagerBuilder().Build();

            var serviceFacade = new S3ServiceFacade()
                .WithConfigurationManager(configurationManager)
                .WithModelService(modelService);

            var modelFactory = new ModelFactoryFacade()
                .WithServiceFacade(serviceFacade)
                .WithPublicationFactory(publicationFactory)
                .Create();

            _model = modelFactory.GetModels(new Context()).Single();
        }

        protected override void Act()
        {
            _assets = _model.Assets;
        }

        [Fact]
        public void ThenItShouldHaveTheAssets()
        {
            _assets.Count().Should().Be(2);

            _assets.Should().Contain(a => a.ID == _asset1.ID);
            _assets.Should().Contain(a => a.ID == _asset2.ID);
        }
    }
}