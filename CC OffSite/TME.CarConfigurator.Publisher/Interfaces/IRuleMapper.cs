using System;
using System.Collections.Generic;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Repository.Objects.Rules;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IRuleMapper
    {
        RuleSets MapCarEquipmentRules(CarEquipmentRules rules);
        RuleSets MapCarPackRules(CarPackRules rules);
    }
}