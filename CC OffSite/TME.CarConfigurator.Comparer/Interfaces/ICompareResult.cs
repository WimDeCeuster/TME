using KellermanSoftware.CompareNetObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.Comparer.Interfaces
{
    public interface ICompareResult
    {
        IReadOnlyList<Difference> Differences { get; }
        IReadOnlyList<Difference> Mismatches { get; }
        IReadOnlyList<Difference> Misorders { get; }
        IReadOnlyList<Difference> Missing { get; }
        IReadOnlyList<Difference> NotImplemented { get; }
        IReadOnlyList<Difference> Exceptions { get; }

        bool Valid { get; }
    }
}