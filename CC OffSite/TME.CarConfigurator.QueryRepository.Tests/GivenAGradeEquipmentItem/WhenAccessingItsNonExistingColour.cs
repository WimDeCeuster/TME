using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using System;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Colours;

namespace TME.CarConfigurator.Query.Tests.GivenAGradeEquipmentItem
{
    public class WhenAccessingItsNonExistingColour : TestBase
    {
        IExteriorColour _colour;
        IGradeEquipmentItem _accessory;

        protected override void Arrange()
        {
            var repoAccessory = new GradeAccessoryBuilder()
                .WithColour(null)
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
        }

        protected override void Act()
        {
            _colour = _accessory.ExteriorColour;
        }

        [Fact]
        public void ThenItShouldHaveNoColour()
        {
            _colour.Should().Be(null);
        }
    }
}
