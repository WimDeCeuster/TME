using TME.CarConfigurator.Repository.Objects.Enums;
using TME.CarConfigurator.Repository.Objects.Rules;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class EquipmentItemRuleBuilder
    {
        readonly EquipmentItemRule _equipmentItem;

        public EquipmentItemRuleBuilder()
        {
            _equipmentItem = new EquipmentItemRule();
        }

        public EquipmentItemRuleBuilder WithCategory(RuleCategory ruleCategory)
        {
            _equipmentItem.Category = ruleCategory;
            return this;
        }

        public EquipmentItemRuleBuilder WithColouringMode(ColouringModes colouringModes)
        {
            _equipmentItem.ColouringModes = colouringModes;
            return this;
        }

        public EquipmentItemRuleBuilder WithShortID(int shortID)
        {
            _equipmentItem.ShortID = shortID;
            return this;
        }

        public EquipmentItemRule Build()
        {
            return _equipmentItem;
        }
    }
}