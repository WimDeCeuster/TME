using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Rules;

namespace TME.CarConfigurator.S3.Publisher
{
    public class RulePublisher : IRulePublisher
    {
        readonly IRuleService _ruleService;

        public RulePublisher(IRuleService ruleService)
        {
            _ruleService = ruleService;
        }

        public async Task PublishCarRulesAsync(IContext context)
        {
            var tasks = context.ContextData.Values.Select(contextData =>
                PublishCarRulesAsync(context.Brand, context.Country, contextData.Publication.ID, contextData.CarRules));

            await Task.WhenAll(tasks);
        }

        async Task PublishCarRulesAsync(string brand, string country, Guid publicationID, IDictionary<Guid, IDictionary<Guid, RuleSets>> carRules)
        {
            var tasks = carRules.Select(entry => _ruleService.PutCarRules(brand, country, publicationID, entry.Key, entry.Value)).ToList();

            await Task.WhenAll(tasks);
        }
    }
}