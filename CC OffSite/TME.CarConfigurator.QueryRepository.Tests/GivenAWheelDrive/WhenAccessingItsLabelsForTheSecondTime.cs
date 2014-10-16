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

namespace TME.CarConfigurator.Query.Tests.GivenAWheelDrive
{
    public class WhenAccessingItsLabelsForTheSecondTime : TestBase
    {
        IWheelDrive _wheelDrive;
        IEnumerable<Interfaces.Core.ILabel> _secondLabels;
        IEnumerable<Interfaces.Core.ILabel> _firstLabels;
        Repository.Objects.Core.Label _label1;
        Repository.Objects.Core.Label _label2;

        protected override void Arrange()
        {
            _label1 = new LabelBuilder()
                .WithCode("code 1")
                .Build();

            _label2 = new LabelBuilder()
                .WithCode("code 2")
                .Build();

            var repoWheelDrive = new WheelDriveBuilder()
                .WithLabels(_label1, _label2)
                .Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            var wheelDriveService = A.Fake<IWheelDriveService>();
            A.CallTo(() => wheelDriveService.GetWheelDrives(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new List<Repository.Objects.WheelDrive> { repoWheelDrive });

            var wheelDriveFactory = new WheelDriveFactoryBuilder()
                .WithWheelDriveService(wheelDriveService)
                .Build();

            _wheelDrive = wheelDriveFactory.GetWheelDrives(publication, context).Single();

            _firstLabels = _wheelDrive.Labels;
        }

        protected override void Act()
        {
            _secondLabels = _wheelDrive.Labels;
        }

        [Fact]
        public void ThenItShouldNotRecalculateTheLabels()
        {
            _secondLabels.Should().BeSameAs(_firstLabels);
        }

        [Fact]
        public void ThenItShouldHaveTheLabels()
        {
            _secondLabels.Count().Should().Be(2);

            _secondLabels.Should().Contain(label => label.Code == _label1.Code);
            _secondLabels.Should().Contain(label => label.Code == _label2.Code);
        }
    }
}
