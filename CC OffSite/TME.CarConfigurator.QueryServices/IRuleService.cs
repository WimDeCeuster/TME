using System;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Rules;

namespace TME.CarConfigurator.QueryServices
{
    public interface IRuleService
    {
        RuleSets GetCarItemRuleSets(Guid itemID, Guid carID, Guid publicationID, Context context);
    }
}