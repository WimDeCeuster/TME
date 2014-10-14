using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAnEngineCategory
{
    public class WhenAccessingItsLabelsForTheSecondTime : TestBase
    {
        IEngineCategory _engineCategory;
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

            var repoEngineCategory = new EngineCategoryBuilder()
                .WithLabels(_label1, _label2)
                .Build();

            _engineCategory = new EngineCategory(repoEngineCategory);

            _firstLabels = _engineCategory.Labels;
        }

        protected override void Act()
        {
            _secondLabels = _engineCategory.Labels;
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
