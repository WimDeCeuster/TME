using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects.Rules;

namespace TME.CarConfigurator.CommandServices
{
    public interface IRuleService
    {
        Task PutCarRules(string brand, string country, Guid publicationID, Guid carID, IDictionary<Guid, RuleSets> carRules);
    }
}