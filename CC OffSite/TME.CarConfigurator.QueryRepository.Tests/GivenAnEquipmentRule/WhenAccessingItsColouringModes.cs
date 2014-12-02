using FluentAssertions;
using TME.CarConfigurator.Interfaces.Rules;
using TME.CarConfigurator.Rules;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;
using ColouringModes = TME.CarConfigurator.Repository.Objects.Enums.ColouringModes;

namespace TME.CarConfigurator.Query.Tests.GivenAnEquipmentRule
{
    public class WhenAccessingItsColouringModes : TestBase
    {
        private IEquipmentRule _equipmentRule;
        private Interfaces.Enums.ColouringModes _adminColouringModes;

        protected override void Arrange()
        {
            var repoEquipmentItemRule = new EquipmentItemRuleBuilder().WithColouringMode(ColouringModes.BodyColour).Build();

            _equipmentRule = new EquipmentRule(repoEquipmentItemRule);
        }

        protected override void Act()
        {
            _adminColouringModes = _equipmentRule.ColouringModes;
        }

        [Fact]
        public void ThenTheCategoryShouldBeCorrect()
        {
            _adminColouringModes.Should().Be(Interfaces.Enums.ColouringModes.BodyColour);
        }
    }
}