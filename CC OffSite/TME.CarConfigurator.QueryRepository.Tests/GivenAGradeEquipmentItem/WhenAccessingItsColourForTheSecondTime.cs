using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAGradeEquipmentItem
{
    public class WhenAccessingItsColourForTheSecondTime : TestBase
    {
        IExteriorColour _firstColour;
        IExteriorColour _secondColour;
        Repository.Objects.Colours.ExteriorColour _repoColour;
        IGradeEquipmentItem _accessory;

        protected override void Arrange()
        {
            _repoColour = new CarConfigurator.Tests.Shared.TestBuilders.ExteriorColourBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoAccessory = new GradeAccessoryBuilder()
                .WithColour(_repoColour)
                .Build();

            var repoGradeEquipment = new GradeEquipmentBuilder()
                .WithAccessories(repoAccessory)
                .Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            var gradeEquipmentService = A.Fake<IGradeEquipmentService>();
            A.CallTo(() => gradeEquipmentService.GetGradeEquipment(A<Guid>._, A<Guid>._, A<Guid>._, A<Context>._)).Returns(repoGradeEquipment);

            var gradeEquipmentFactory = new GradeEquipmentFactoryBuilder()
                .WithGradeEquipmentService(gradeEquipmentService)
                .Build();

            _accessory = gradeEquipmentFactory.GetGradeEquipment(publication, context, Guid.Empty).GradeAccessories.Single();

            _firstColour = _accessory.ExteriorColour;
        }

        protected override void Act()
        {
            _secondColour = _accessory.ExteriorColour;
        }

        [Fact]
        public void ThenItShouldNotRecalculateTheColour()
        {
            _secondColour.Should().Be(_firstColour);
        }

        [Fact]
        public void ThenItShouldHaveTheColour()
        {
            _secondColour.ID.Should().Be(_repoColour.ID);
        }


    }
}
