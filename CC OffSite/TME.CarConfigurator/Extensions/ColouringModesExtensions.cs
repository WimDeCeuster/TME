using System;
using TME.CarConfigurator.Interfaces.Enums;
using RepoColouringModes = TME.CarConfigurator.Repository.Objects.Enums.ColouringModes;

namespace TME.CarConfigurator.Extensions
{
    internal static class ColouringModesExtensions
    {
        internal static ColouringModes Convert(this RepoColouringModes colourModes)
        {
            if ((colourModes & ~RepoColouringModes.SecondaryAccentColour & ~RepoColouringModes.PrimaryAccentColour & ~RepoColouringModes.BodyColour & ~RepoColouringModes.None) != 0)
                throw new ArgumentException(String.Format("Unrecognised colouring mode flag in {0}", colourModes.ToString("g")));

            ColouringModes result = 0;
            if (colourModes.HasFlag(RepoColouringModes.SecondaryAccentColour))
                result |= ColouringModes.SecondaryAccentColour;
            if (colourModes.HasFlag(RepoColouringModes.PrimaryAccentColour))
                result |= ColouringModes.PrimaryAccentColour;
            if (colourModes.HasFlag(RepoColouringModes.BodyColour))
                result |= ColouringModes.BodyColour;

            return result;
        }
    }
}
