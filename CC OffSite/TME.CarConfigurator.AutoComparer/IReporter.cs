using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TME.CarConfigurator.AutoComparer
{
    public interface IReporter
    {
        void WriteReport(AutoCompareResult result, string path = "");
    }
}
