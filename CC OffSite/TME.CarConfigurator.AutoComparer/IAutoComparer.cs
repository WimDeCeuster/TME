using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TME.CarConfigurator.Publisher.Common.Enums;

namespace TME.CarConfigurator.AutoComparer
{
    public interface IAutoComparer
    {
        AutoCompareResult Compare(IList<string> countries, String brand, PublicationDataSubset dataSubset);
    }
}
