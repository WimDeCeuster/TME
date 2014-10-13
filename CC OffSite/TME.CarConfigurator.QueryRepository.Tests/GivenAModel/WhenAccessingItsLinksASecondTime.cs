using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.DI;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Enums;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using TME.CarConfigurator.Tests.Shared.TestBuilders.RepositoryObjects;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAModel
{
    public class WhenAccessingItsLinksASecondTime : TestBase
    {
        private Guid _publicationId;
        private IModel _model;
        private IEnumerable<ILink> _firstLinks;
        private IEnumerable<ILink> _secondLinks;
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

            var serviceFacade = new S3ServiceFacade()
                .WithModelService(modelService);

            var modelFactory = new ModelFactoryFacade()
                .WithServiceFacade(serviceFacade)
                .WithPublicationFactory(publicationFactory)
                .Create();

            _model = modelFactory.GetModels(new Context()).Single();

            _firstLinks = _model.Links;
        }

        protected override void Act()
        {
            _secondLinks = _model.Links;
        }

        [Fact]
        public void ThenItShouldNotRecalculateTheLinks()
        {
            _secondLinks.Should().BeSameAs(_firstLinks);
        }

        [Fact]
        public void ThenItShouldHaveTheLinks()
        {
            _secondLinks.Count().Should().Be(2);

            _secondLinks.Should().Contain(l => l.ID == _link1.ID);
            _secondLinks.Should().Contain(l => l.ID == _link2.ID);
        }
    }
}