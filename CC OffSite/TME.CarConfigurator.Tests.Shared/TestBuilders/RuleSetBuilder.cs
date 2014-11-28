using TME.CarConfigurator.Repository.Objects.Rules;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class RuleSetBuilder
    {
        readonly RuleSet _ruleSet;

        public RuleSetBuilder()
        {
            _ruleSet = new RuleSet();
        }

        public RuleSet Build()
        {
            return _ruleSet;
        }
    }
}