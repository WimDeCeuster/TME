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

namespace TME.CarConfigurator.Query.Tests.GivenAGradeOption
{
    public class WhenAccessingItsNonExistingParentOptionInfo : TestBase
    {
        IOptionInfo _optionInfo;
        IGradeOption _option;

        protected override void Arrange()
        {
            var repoOption = new GradeOptionBuilder()
                .Build();

            var repoGradeEquipment = new GradeEquipmentBuilder()
                .WithOptions(repoOption)
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

            _option = gradeEquipmentFactory.GetGradeEquipment(publication, context, Guid.Empty).Options.Single();
        }

        protected override void Act()
        {
            _optionInfo = _option.ParentOption;
        }

        [Fact]
        public void ThenItShouldHaveTheOptionInfo()
        {
            _optionInfo.Should().Be(null);
        }
    }
}
