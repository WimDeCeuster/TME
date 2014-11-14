using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Comparer.Interfaces;

namespace TME.CarConfigurator.AutoComparer
{
    public class ModelCompareResult
    {
        public String ModelName { get; private set; }
        public ICompareResult Result { get; private set; }

        public ModelCompareResult(String modelName, ICompareResult result)
        {
            ModelName = modelName;
            Result = result;
        }
    }
}
