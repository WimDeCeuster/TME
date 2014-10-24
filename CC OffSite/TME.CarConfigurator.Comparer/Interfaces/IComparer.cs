using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces;

namespace TME.CarConfigurator.Comparer.Interfaces
{
    public interface IComparer
    {
        ICompareResult Compare(IModel modelA, IModel modelB);
    }
}
