using KellermanSoftware.CompareNetObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Comparer.Interfaces;

namespace TME.CarConfigurator.Comparer
{
    public class CompareResult : ICompareResult
    {
        public IReadOnlyList<Difference> Differences { get; private set; }
        public IReadOnlyList<Difference> Mismatches { get; private set; }
        public IReadOnlyList<Difference> Misorders { get; private set; }
        public IReadOnlyList<Difference> Missing { get; private set; }
        public IReadOnlyList<Difference> NotImplemented { get; private set; }
        public IReadOnlyList<Difference> Exceptions { get; private set; }

        public bool Valid { get; private set; }

        public CompareResult(IEnumerable<Difference> differences)
        {
            Differences = differences.ToList();

            Mismatches = Differences.Where(difference => difference.Type == DifferenceType.Mismatch).ToList();
            Misorders = Differences.Where(difference => difference.Type == DifferenceType.Misorder).ToList();
            Missing = Differences.Where(difference => difference.Type == DifferenceType.Missing).ToList();
            NotImplemented = Differences.Where(difference => difference.Type == DifferenceType.NotImplemented).ToList();
            Exceptions = Differences.Where(difference => difference.Type == DifferenceType.Exception).ToList();

            Valid = Mismatches.Count + Misorders.Count + Missing.Count + NotImplemented.Count + Exceptions.Count == 0;
        }
    }
}
