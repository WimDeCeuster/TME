using TME.CarConfigurator.Interfaces.Enums;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter.Extensions
{
    internal static class VisibilityExtensions
    {

        public static Visibility ToVisibility(this Legacy.ItemVisibility visibility)
        {
            if (visibility == Legacy.ItemVisibility.CarConfigurator) return Visibility.CarConfigurator;
            if (visibility == Legacy.ItemVisibility.Web) return Visibility.Web;
            if (visibility == Legacy.ItemVisibility.Brochure) return Visibility.Brochure;
            return Visibility.None;
        }
    }
}
