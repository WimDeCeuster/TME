using System;
using TME.CarConfigurator.Interfaces.Enums;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter.Extensions
{
    internal static class ColouringModesExtensions
    {
        public static ColouringModes ToColouringModes(this Legacy.ColouringModes colouringModes)
        {
            if ((colouringModes & ~Legacy.ColouringModes.BodyColour & ~Legacy.ColouringModes.PrimaryAccentColour & ~Legacy.ColouringModes.SecondaryAccentColour & ~Legacy.ColouringModes.None) != 0)
                throw new ArgumentException(String.Format("Unrecognised colouringModes flag in {0}", colouringModes.ToString("g")));

            var result = ColouringModes.None;
            if (colouringModes.HasFlag(Legacy.ColouringModes.BodyColour))
                result |= ColouringModes.BodyColour;
            if (colouringModes.HasFlag(Legacy.ColouringModes.PrimaryAccentColour))
                result |= ColouringModes.PrimaryAccentColour;
            if (colouringModes.HasFlag(Legacy.ColouringModes.SecondaryAccentColour))
                result |= ColouringModes.SecondaryAccentColour;

            return result;
        }
    }
}
