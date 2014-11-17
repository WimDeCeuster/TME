using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KellermanSoftware.CompareNetObjects
{
    public enum DifferenceType
    {
        Mismatch = 0,
        Missing = 1,
        Misorder = 2,
        Exception = 3,
        NotImplemented = 4
    }
}
