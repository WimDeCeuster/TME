using FakeItEasy;
using TME.CarConfigurator.Factories;
using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
{
    public class RuleFactoryBuilder
    {
        private IRuleService _ruleService = A.Fake<IRuleService>();

        public RuleFactoryBuilder WithRuleService(IRuleService ruleService)
        {
            _ruleService = ruleService;
            return this;
        }

        public RuleFactory Build()
        {
            return new RuleFactory(_ruleService);
        }
    }
}