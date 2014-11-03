using System;
using FakeItEasy;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.Query.Tests.GivenAGradeEquipmentItem
{
    public class WhenAccessingItsLinksForTheSecondTime : TestBase
    {
        IGradeEquipmentItem _gradeEquipmentItem;
        IEnumerable<Interfaces.ILink> _secondLinks;
        IEnumerable<Interfaces.ILink> _firstLinks;
        Repository.Objects.Link _link1;
        Repository.Objects.Link _link2;

        protected override void Arrange()
        {
            _link1 = new LinkBuilder()
                .WithId(5)
                .Build();

            _link2 = new LinkBuilder()
                .WithId(8)
                .Build();

            var repoGradeEquipmentItem = new GradeAccessoryBuilder()
                .WithLinks(_link1, _link2)
                .Build();

            var repoGradeEquipment = new GradeEquipmentBuilder()
                .WithAccessories(repoGradeEquipmentItem)
                .Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            var gradeEquipmentService = A.Fake<IEquipmentService>();
            A.CallTo(() => gradeEquipmentService.GetGradeEquipment(A<Guid>._, A<Guid>._, A<Guid>._, A<Context>._)).Returns(repoGradeEquipment);

            var gradeEquipmentFactory = new EquipmentFactoryBuilder()
                .WithEquipmentService(gradeEquipmentService)
                .Build();

            _gradeEquipmentItem = gradeEquipmentFactory.GetGradeEquipment(publication, context, Guid.Empty).Accessories.Single();

            _firstLinks = _gradeEquipmentItem.Links;
        }

        protected override void Act()
        {
            _secondLinks = _gradeEquipmentItem.Links;
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

            _secondLinks.Should().Contain(link => link.ID == _link1.ID);
            _secondLinks.Should().Contain(link => link.ID == _link2.ID);
        }
    }
}
