using System;
using TME.CarConfigurator.Interfaces.Enums;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter.Extensions
{
    internal static class VisibilityExtensions
    {
        public static Visibility ToVisibility(this Legacy.ItemVisibility visibility)
        {
            if ((visibility & ~Legacy.ItemVisibility.Web & ~Legacy.ItemVisibility.CarConfigurator & ~Legacy.ItemVisibility.Brochure & ~Legacy.ItemVisibility.None) != 0)
                throw new ArgumentException(String.Format("Unrecognised visibility flag in {0}", visibility.ToString("g")));

            Visibility result = 0;
            if (visibility.HasFlag(Legacy.ItemVisibility.Web))
                result |= Visibility.Web;
            if (visibility.HasFlag(Legacy.ItemVisibility.CarConfigurator))
                result |= Visibility.CarConfigurator;
            if (visibility.HasFlag(Legacy.ItemVisibility.Brochure))
                result |= Visibility.Brochure;

            return result;
        }
    }
}
