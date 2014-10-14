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
    public class WhenAccessingItsLinks : TestBase
    {
        private Guid _publicationId;
        private IModel _model;
        private IEnumerable<ILink> _links;
        private Repository.Objects.Link _link1;
        private Repository.Objects.Link _link2;

        protected override void Arrange()
        {
            _link1 = new LinkBuilder()
                .WithId(12)
                .Build();

            _link2 = new LinkBuilder()
                .WithId(45)
                .Build();

            _publicationId = Guid.NewGuid();
            var generation = new GenerationBuilder()
                .AddLink(_link1)
                .AddLink(_link2)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(_publicationId)
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
            _links = _model.Links;
        }

        [Fact]
        public void ThenItShouldHaveTheLinks()
        {
            _links.Count().Should().Be(2);

            _links.Should().Contain(l => l.ID == _link1.ID);
            _links.Should().Contain(l => l.ID == _link2.ID);
        }
    }
}