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
        public IDictionary<String, Boolean> Paths { get; private set; }
        public string Result { get; set; }

        public CompareResult()
        {
            Paths = new Dictionary<String, Boolean>();
        }
    }
}
