using TME.CarConfigurator.Interfaces.Enums;
using TME.CarConfigurator.Interfaces.Rules;
using TME.CarConfigurator.Tests.Shared;

namespace TME.CarConfigurator.Query.Tests.GivenAnEquipmentRule
{
    public class WhenAccessingItsCategoryForTheFirstTime : TestBase
    {
        private IEquipmentRule _equipmentRule;
        private RuleCategory _equipmentRuleCategory;

        protected override void Arrange()
        {
            throw new System.NotImplementedException();
        }

        protected override void Act()
        {
            _equipmentRuleCategory = _equipmentRule.Category;
        }
    }
}