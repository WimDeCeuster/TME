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
    public class WhenAccessingItsParentOptionInfoForTheFirstTime : TestBase
    {
        IOptionInfo _optionInfo;
        IReadOnlyList<IGradeOption> _options;
        const int _parentShortId = 5;

        protected override void Arrange()
        {
            var parentRepoOption = new GradeOptionBuilder()
                .WithShortId(_parentShortId)
                .Build();

            var childRepoOption = new GradeOptionBuilder()
                .WithParentShortId(_parentShortId)
                .Build();

            var repoGradeEquipment = new GradeEquipmentBuilder()
                .WithOptions(childRepoOption, parentRepoOption)
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

            _options = gradeEquipmentFactory.GetGradeEquipment(publication, context, Guid.Empty).Options;
        }

        protected override void Act()
        {
            _optionInfo = _options.Select(option => option.ParentOption).Single(optionInfo => optionInfo != null);
        }

        [Fact]
        public void ThenItShouldHaveTheOptionInfo()
        {
            _optionInfo.ShortID.Should().Be(_parentShortId);
        }
    }
}
