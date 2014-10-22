using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenASubModel
{
    public class WhenAccessingItsLabelsForTheSecondTime : TestBase
    {
        private ISubModel _subModel;
        private IEnumerable<ILabel> _secondLabels;
        private Repository.Objects.Core.Label _label1;
        private Repository.Objects.Core.Label _label2;
        private IEnumerable<ILabel> _firstLabels;

        protected override void Arrange()
        {
            _label1 = new LabelBuilder().WithCode("code for label 1").Build();
            _label2 = new LabelBuilder().WithCode("code for label 2").Build();

            var repositorySubModel = new SubModelBuilder()
                .WithID(Guid.NewGuid())
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

            var subModelService = A.Fake<ISubModelService>();
            A.CallTo(() => subModelService.GetSubModels(A<Guid>._, A<Guid>._, A<Context>._))
                .Returns(new List<Repository.Objects.SubModel> { repositorySubModel });

            var subModelFactory = new SubModelFactoryBuilder()
                .WithSubModelService(subModelService)
                .Build();

            _subModel = subModelFactory.GetSubModels(publication, context).Single();

            _firstLabels = _subModel.Labels;
        }

        protected override void Act()
        {
            _secondLabels = _subModel.Labels;
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