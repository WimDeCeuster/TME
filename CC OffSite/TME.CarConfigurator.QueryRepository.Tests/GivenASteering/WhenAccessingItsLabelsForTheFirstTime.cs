using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenASteering
{
    public class WhenAccessingItsLabelsForTheFirstTime : TestBase
    {
        ISteering _steering;
        IEnumerable<Interfaces.Core.ILabel> _labels;
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

            var repoSteering = new SteeringBuilder()
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

            var steeringService = A.Fake<ISteeringService>();
            A.CallTo(() => steeringService.GetSteerings(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new List<Repository.Objects.Steering> { repoSteering });

            var steeringFactory = new SteeringFactoryBuilder()
                .WithSteeringService(steeringService)
                .Build();

            _steering = steeringFactory.GetSteerings(publication, context).Single();
        }

        protected override void Act()
        {
            _labels = _steering.Labels;
        }

        [Fact]
        public void ThenItShouldHaveTheLabels()
        {
            _labels.Count().Should().Be(2);

            _labels.Should().Contain(label => label.Code == _label1.Code);
            _labels.Should().Contain(label => label.Code == _label2.Code);
        }
    }
}
