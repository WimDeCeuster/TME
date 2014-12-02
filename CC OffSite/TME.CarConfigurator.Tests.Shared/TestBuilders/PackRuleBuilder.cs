using TME.CarConfigurator.Repository.Objects.Enums;
using TME.CarConfigurator.Repository.Objects.Rules;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class PackRuleBuilder
    {
        readonly PackRule _packRule;

        public PackRuleBuilder()
        {
            _packRule = new PackRule();
        }

        public PackRuleBuilder WithCategory(RuleCategory ruleCategory)
        {
            _packRule.Category = ruleCategory;
            return this;    
        }

        public PackRuleBuilder WithShortID(int shortID)
        {
            _packRule.ShortID = shortID;
            return this;
        }

        public PackRule Build()
        {
            return _packRule;
        }
    }
}