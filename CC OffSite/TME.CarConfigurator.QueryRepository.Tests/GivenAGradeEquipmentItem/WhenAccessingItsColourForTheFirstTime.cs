using System.Linq;
using FakeItEasy;
using FluentAssertions;
using System;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.Query.Tests.GivenAGradeEquipmentItem
{
    public class WhenAccessingItsColourForTheFirstTime : TestBase
    {
        IExteriorColour _colour;
        ExteriorColour _repoColour;
        IGradeEquipmentItem _accessory;

        protected override void Arrange()
        {
            _repoColour = new EquipmentExteriorColourBuilder()
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

            var gradeEquipmentService = A.Fake<IEquipmentService>();
            A.CallTo(() => gradeEquipmentService.GetGradeEquipment(A<Guid>._, A<Guid>._, A<Guid>._, A<Context>._)).Returns(repoGradeEquipment);

            var colourFactory = new ColourFactoryBuilder().Build();

            var gradeEquipmentFactory = new EquipmentFactoryBuilder()
                .WithEquipmentService(gradeEquipmentService)
                .WithColourFactory(colourFactory)
                .Build();

            _accessory = gradeEquipmentFactory.GetGradeEquipment(publication, context, Guid.Empty).Accessories.Single();
        }

        protected override void Act()
        {
            _colour = _accessory.ExteriorColour;
        }

        [Fact]
        public void ThenItShouldHaveTheColour()
        {
            _colour.ID.Should().Be(_repoColour.ID);
        }
    }
}
