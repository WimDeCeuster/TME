using TME.CarConfigurator.Administration.Enums;
using TME.CarConfigurator.Publisher.Exceptions;
using TME.CarConfigurator.Repository.Objects.Enums;

namespace TME.CarConfigurator.Publisher.Extensions
{
    public static class VisibilityExtensions
    {
        public static Visibility Convert(this ItemVisibility visibility)
        {
            if ((visibility & ~ItemVisibility.Website & ~ItemVisibility.CarConfigurator & ~ItemVisibility.Brochure & ~ItemVisibility.None) != 0)
                throw new UnrecognisedItemVisibilityException(visibility);

            Visibility result = 0;
            if (visibility.HasFlag(ItemVisibility.Website))
                result |= Visibility.Web;
            if (visibility.HasFlag(ItemVisibility.CarConfigurator))
                result |= Visibility.CarConfigurator;
            if (visibility.HasFlag(ItemVisibility.Brochure))
                result |= Visibility.Brochure;

            return result;
        }
    }
}
