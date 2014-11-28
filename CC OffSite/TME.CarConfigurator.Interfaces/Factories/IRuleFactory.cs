using System;
using TME.CarConfigurator.Interfaces.Rules;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface IRuleFactory
    {
        IRuleSets GetCarPackRuleSets(Guid itemID, Guid carID, Publication publication, Context context);
        IRuleSets GetCarEquipmentRuleSets(Guid itemID, Guid carID, Publication publication, Context repositoryContext);
    }
}