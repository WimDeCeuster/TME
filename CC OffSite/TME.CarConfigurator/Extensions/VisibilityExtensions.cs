using System;
using TME.CarConfigurator.Interfaces.Enums;
using RepoVisibility = TME.CarConfigurator.Repository.Objects.Enums.Visibility;

namespace TME.CarConfigurator.Extensions
{
    internal static class VisibilityExtensions
    {
        public static Visibility ToVisibility(this RepoVisibility visibility)
        {
            if ((visibility & ~RepoVisibility.Web & ~RepoVisibility.CarConfigurator & ~RepoVisibility.Brochure & ~RepoVisibility.None) != 0)
                throw new ArgumentException(String.Format("Unrecognised visibility flag in {0}", visibility.ToString("g")));

            Visibility result = 0;
            if (visibility.HasFlag(RepoVisibility.Web))
                result |= Visibility.Web;
            if (visibility.HasFlag(RepoVisibility.CarConfigurator))
                result |= Visibility.CarConfigurator;
            if (visibility.HasFlag(RepoVisibility.Brochure))
                result |= Visibility.Brochure;

            return result;
        }
    }
}
