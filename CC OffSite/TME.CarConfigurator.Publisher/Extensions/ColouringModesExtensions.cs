using TME.CarConfigurator.Administration.Enums;
using TME.CarConfigurator.Publisher.Exceptions;

namespace TME.CarConfigurator.Publisher.Extensions
{
    public static class ColouringModesExtensions
    {
        public static Repository.Objects.Enums.ColouringModes Convert(this ColouringModes modes)
        {
            if ((modes & ~ColouringModes.SecondaryAccentColour & ~ColouringModes.PrimaryAccentColour & ~ColouringModes.BodyColour & ~ColouringModes.None) != 0)
                throw new UnrecognisedColouringModeException(modes);

            Repository.Objects.Enums.ColouringModes result = 0;
            if (modes.HasFlag(ColouringModes.SecondaryAccentColour))
                result |= Repository.Objects.Enums.ColouringModes.SecondaryAccentColour;
            if (modes.HasFlag(ColouringModes.PrimaryAccentColour))
                result |= Repository.Objects.Enums.ColouringModes.PrimaryAccentColour;
            if (modes.HasFlag(ColouringModes.BodyColour))
                result |= Repository.Objects.Enums.ColouringModes.BodyColour;

            return result;
        }
    }
}
