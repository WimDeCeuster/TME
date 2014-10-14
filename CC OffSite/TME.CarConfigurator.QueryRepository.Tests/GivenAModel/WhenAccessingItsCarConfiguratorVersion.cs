using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.DI;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Enums;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAModel
{
    public class WhenAccessingItsCarConfiguratorVersion : TestBase
    {
        private IModel _model;
        private ICarConfiguratorVersion _actualVersion;
        private const short CcVersionId = 23;
        private const string CcVersionName = "cc version";

        protected override void Arrange()
        {
            var generation = new GenerationBuilder()
                .WithCarConfiguratorVersion(CcVersionId, CcVersionName)
                .Build();

            var publication = new PublicationBuilder()
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
            _actualVersion = _model.CarConfiguratorVersion;
        }

        [Fact]
        public void ThenItShouldHaveTheCarConfiguratorVersion()
        {
            _actualVersion.ID.Should().Be(CcVersionId);
            _actualVersion.Name.Should().Be(CcVersionName);
        }
    }
}