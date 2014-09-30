using System;

namespace TME.CarConfigurator.Repository.Objects.Assets.Enums
{
    [Serializable, Flags]
    public enum Scope
    {
        Public = 1,
        Internal = 2
    }
}
