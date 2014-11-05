using System;

namespace TME.CarConfigurator.Interfaces.Enums
{
    [Serializable, Flags]
    public enum ColouringModes
    {
        None = 0,
        BodyColour = 1,
        PrimaryAccentColour = 2,
        SecondaryAccentColour = 4
    }
}
