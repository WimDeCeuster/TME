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
        public IReadOnlyList<String> Differences { get; private set; }
        public IReadOnlyList<String> Mismatches { get; private set; }
        public IReadOnlyList<String> Misorders { get; private set; }
        public IReadOnlyList<String> Missing { get; private set; }
        public IReadOnlyList<String> NotImplemented { get; private set; }
        public IReadOnlyList<String> Exceptions { get; private set; }

        public bool IsValid { get; private set; }

        public CompareResult(IList<Difference> differences)
        {
            Differences = differences.Select(difference => difference.ToString()).ToList();

            Mismatches = differences.Where(difference => difference.Type == DifferenceType.Mismatch).Select(difference => difference.ToString()).ToList();
            Misorders = differences.Where(difference => difference.Type == DifferenceType.Misorder).Select(difference => difference.ToString()).ToList();
            Missing = differences.Where(difference => difference.Type == DifferenceType.Missing).Select(difference => difference.ToString()).ToList();
            NotImplemented = differences.Where(difference => difference.Type == DifferenceType.NotImplemented).Select(difference => difference.ToString()).ToList();
            Exceptions = differences.Where(difference => difference.Type == DifferenceType.Exception).Select(difference => difference.ToString()).ToList();

            IsValid = Mismatches.Count + Misorders.Count + Missing.Count + NotImplemented.Count + Exceptions.Count == 0;
        }
    }
}
