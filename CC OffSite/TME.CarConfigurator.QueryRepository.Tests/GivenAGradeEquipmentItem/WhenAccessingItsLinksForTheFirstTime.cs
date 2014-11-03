using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAGradeEquipmentItem
{
    public class WhenAccessingItsLinksForTheFirstTime : TestBase
    {
        IGradeEquipmentItem _gradeEquipmentItem;
        IEnumerable<Interfaces.ILink> _links;
        Repository.Objects.Link _link1;
        Repository.Objects.Link _link2;

        protected override void Arrange()
        {
            _link1 = new LinkBuilder()
                .WithId(8)
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
        }

        protected override void Act()
        {
            _links = _gradeEquipmentItem.Links;
        }

        [Fact]
        public void ThenItShouldHaveTheLinks()
        {
            _links.Count().Should().Be(2);

            _links.Should().Contain(link => link.ID == _link1.ID);
            _links.Should().Contain(link => link.ID == _link2.ID);
        }
    }
}
