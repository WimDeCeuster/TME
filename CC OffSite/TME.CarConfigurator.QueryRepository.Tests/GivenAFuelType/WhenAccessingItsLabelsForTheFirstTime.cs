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

namespace TME.CarConfigurator.Query.Tests.GivenAFuelType
{
    public class WhenAccessingItsLabelsForTheFirstTime : TestBase
    {
        IFuelType _fuelType;
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

            var repoFuelType = new FuelTypeBuilder()
                .WithLabels(_label1, _label2)
                .Build();

            _fuelType = new FuelType(repoFuelType);
        }

        protected override void Act()
        {
            _labels = _fuelType.Labels;
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
