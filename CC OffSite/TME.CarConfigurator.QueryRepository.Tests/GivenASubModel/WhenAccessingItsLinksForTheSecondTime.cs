using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenASubModel
{
    public class WhenAccessingItsLinksForTheSecondTime : TestBase
    {
        private ISubModel _subModel;
        private IEnumerable<ILink> _secondLinks;
        private Repository.Objects.Link _link1;
        private Repository.Objects.Link _link2;
        private IEnumerable<ILink> _firstLinks;

        protected override void Arrange()
        {
            _link1 = new LinkBuilder().WithId(10).Build();
            _link2 = new LinkBuilder().WithId(35).Build();

            var repositorySubModel = new SubModelBuilder()
                .WithID(Guid.NewGuid())
                .WithLinks(_link1, _link2)
                .Build();

            var publicationTimeFrame =
                new PublicationTimeFrameBuilder()
                .WithID(Guid.NewGuid())
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            var subModelService = A.Fake<ISubModelService>();
            A.CallTo(() => subModelService.GetSubModels(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new List<Repository.Objects.SubModel>() { repositorySubModel });

            var subModelFactory = new SubModelFactoryBuilder()
                .WithSubModelService(subModelService)
                .Build();

            _subModel = subModelFactory.GetSubModels(publication, context).Single();

            _firstLinks = _subModel.Links;
        }

        protected override void Act()
        {
            _secondLinks = _subModel.Links;
        }

        [Fact]
        public void ThenItShouldNotRecalculateTheLinks()
        {
            _secondLinks.Should().BeSameAs(_firstLinks);
        }

        [Fact]
        public void ThenItShouldHaveTheCorrectLinks()
        {
            _secondLinks.Count().Should().Be(2);

            _secondLinks.Should().Contain(l => l.ID == _link1.ID);
            _secondLinks.Should().Contain(l => l.ID == _link2.ID);
        }
    }
}