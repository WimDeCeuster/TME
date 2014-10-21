using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.QueryServices;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenASubModel
{
    public class WhenAccessingItsLinksForTheFirstTime : TestBase
    {
        private ISubModel _subModel;
        private IEnumerable<ILink> _links;
        private Repository.Objects.Link _link1;
        private Repository.Objects.Link _link2;

        protected override void Arrange()
        {
            _link1 = new LinkBuilder().WithId(10).Build();
            _link2 = new LinkBuilder().WithId(35).Build();

            var repositorySubModel = new SubModelBuilder()
                .WithID(Guid.NewGuid())
                .WithLinks(_link1,_link2)
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
            A.CallTo(() => subModelService.GetSubModels(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new List<Repository.Objects.SubModel>() {repositorySubModel});

            var subModelFactory = new SubModelFactoryBuilder()
                .WithSubModelService(subModelService)
                .Build();

            _subModel = subModelFactory.GetSubModels(publication, context).Single();
        }

        protected override void Act()
        {
            _links = _subModel.Links;
        }

        [Fact]
        public void ThenItShouldHaveTheLabels()
        {
            _links.Count().Should().Be(2);

            _links.Should().Contain(l => l.ID == _link1.ID);
            _links.Should().Contain(l => l.ID == _link2.ID);
        }
    }
}