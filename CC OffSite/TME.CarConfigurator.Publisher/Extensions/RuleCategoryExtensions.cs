using TME.CarConfigurator.Publisher.Exceptions;
using TME.CarConfigurator.Repository.Objects.Enums;

namespace TME.CarConfigurator.Publisher.Extensions
{
    public static class RuleCategoryExtensions
    {
        public static RuleCategory Convert(this Administration.Enums.RuleCategory category)
        {
            switch (category)
            {
                case Administration.Enums.RuleCategory.MARKETING:
                    return RuleCategory.Marketing;
                case Administration.Enums.RuleCategory.PRODUCT:
                    return RuleCategory.Product;
                case Administration.Enums.RuleCategory.VISUAL:
                    return RuleCategory.Visual;
                case 0:
                    return RuleCategory.None;
                default:
                    throw new UnrecognisedRuleCategoryException(category);
            }
        }
    }
}
