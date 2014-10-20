using System;

namespace TME.CarConfigurator.Interfaces.Enums
{
    [Serializable, Flags]
    public enum Visibility
    {
        None = 0,
        Web = 1,
        CarConfigurator = 2,
        Brochure = 4
    }
}
