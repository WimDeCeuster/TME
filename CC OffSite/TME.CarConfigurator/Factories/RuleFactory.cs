using System;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Interfaces.Rules;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Rules;

namespace TME.CarConfigurator.Factories
{
    public class RuleFactory : IRuleFactory
    {
        private readonly IRuleService _ruleService;

        public RuleFactory(IRuleService ruleService)
        {
            if (ruleService == null) throw new ArgumentNullException("ruleService");

            _ruleService = ruleService;
        }

        public IRuleSets GetCarPackRuleSets(Guid itemID, Guid carID, Publication publication, Context context)
        {
             return new RuleSets(_ruleService.GetCarItemRuleSets(itemID, carID, publication.ID, context));
        }

        public IRuleSets GetCarEquipmentRuleSets(Guid itemID, Guid carID, Publication publication, Context context)
        {
            return new RuleSets(_ruleService.GetCarItemRuleSets(itemID, carID, publication.ID, context));
        }
    }
}