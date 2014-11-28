using System;
using TME.CarConfigurator.Interfaces.Enums;

namespace TME.CarConfigurator.Extensions
{
    public static class RuleCategoryExtensions
    {
        public static RuleCategory Convert(this Repository.Objects.Enums.RuleCategory category)
        {
            switch (category)
            {
                case Repository.Objects.Enums.RuleCategory.Marketing:
                    return RuleCategory.Marketing;
                case Repository.Objects.Enums.RuleCategory.Product:
                    return RuleCategory.Product;
                case Repository.Objects.Enums.RuleCategory.Visual:
                    return RuleCategory.Visual;
                default :
                    throw new Exception(String.Format("error on {0}", category));
            }
        }
    }
}