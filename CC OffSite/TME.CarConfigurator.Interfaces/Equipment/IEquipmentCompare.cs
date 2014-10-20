using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.Interfaces.Equipment
{
    public interface IEquipmentCompare
    {
        bool Standard { get; }
        bool Optional { get; }
        bool NotAvailable { get; }

        IEnumerable<ICarInfo> StandardOn { get; }
        IEnumerable<ICarInfo> OptionalOn { get; }
        IEnumerable<ICarInfo> NotAvailableOn { get; }
    }
}
