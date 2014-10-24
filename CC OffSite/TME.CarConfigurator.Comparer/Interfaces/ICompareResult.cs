using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.Comparer.Interfaces
{
    public interface ICompareResult
    {
        IDictionary<String, Boolean> Paths { get; }
        string Result { get; set; }
    }
}
