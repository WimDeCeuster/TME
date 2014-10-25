using System.Collections.Generic;

namespace TME.CarConfigurator.Interfaces
{
    public interface IAvailabilityCompare
    {
        bool Standard { get; }
        bool Optional { get; }
        bool NotAvailable { get; }

        IEnumerable<ICarInfo> StandardOn { get; }
        IEnumerable<ICarInfo> OptionalOn { get; }
        IEnumerable<ICarInfo> NotAvailableOn { get; }
    }
}