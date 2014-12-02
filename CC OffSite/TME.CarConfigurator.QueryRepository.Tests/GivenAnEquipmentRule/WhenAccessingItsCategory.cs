using FluentAssertions;
using TME.CarConfigurator.Interfaces.Enums;
using TME.CarConfigurator.Interfaces.Rules;
using TME.CarConfigurator.Rules;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAnEquipmentRule
{
    public class WhenAccessingItsCategory : TestBase
    {
        private IEquipmentRule _equipmentRule;
        private RuleCategory _equipmentRuleCategory;

        protected override void Arrange()
        {
            var repoEquipmentItemRule = new EquipmentItemRuleBuilder().WithCategory(Repository.Objects.Enums.RuleCategory.Marketing).Build();

            _equipmentRule = new EquipmentRule(repoEquipmentItemRule);
        }

        protected override void Act()
        {
            _equipmentRuleCategory = _equipmentRule.Category;
        }

        [Fact]
        public void ThenTheCategoryShouldBeCorrect()
        {
            _equipmentRuleCategory.Should().Be(RuleCategory.Marketing);
        }
    }
}