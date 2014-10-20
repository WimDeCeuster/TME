using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TME.CarConfigurator.Repository.Objects.Enums
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
