using System;
using System.Collections.Generic;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Repository.Objects.Rules;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IRuleMapper
    {
        IDictionary<Guid, RuleSets> MapCarRules(Car car, EquipmentGroups equipmentGroups, EquipmentItems equipmentItems);
    }
}