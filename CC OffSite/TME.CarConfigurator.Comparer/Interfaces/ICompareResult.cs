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
        IReadOnlyList<String> Differences { get; }
        IReadOnlyList<String> Mismatches { get; }
        IReadOnlyList<String> Misorders { get; }
        IReadOnlyList<String> Missing { get; }
        IReadOnlyList<String> NotImplemented { get; }
        IReadOnlyList<String> Exceptions { get; }

        bool IsValid { get; }
    }
}