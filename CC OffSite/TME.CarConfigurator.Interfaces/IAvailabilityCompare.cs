using System.Collections.Generic;

namespace TME.CarConfigurator.Interfaces
{
    public interface IAvailabilityCompare
    {
        bool Standard { get; }
        bool Optional { get; }
        bool NotAvailable { get; }

        IReadOnlyList<ICarInfo> StandardOn { get; }
        IReadOnlyList<ICarInfo> OptionalOn { get; }
        IReadOnlyList<ICarInfo> NotAvailableOn { get; }
    }
}