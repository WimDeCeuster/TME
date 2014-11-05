using System.Collections.Generic;

namespace TME.CarConfigurator.Interfaces
{
    public interface IAvailabilityCompare
    {
        bool Standard { get; }
        bool Optional { get; }
        bool NotAvailable { get; }

        // ReSharper disable ReturnTypeCanBeEnumerable.Global
        IReadOnlyList<ICarInfo> StandardOn { get; }
        IReadOnlyList<ICarInfo> OptionalOn { get; }
        IReadOnlyList<ICarInfo> NotAvailableOn { get; }
        // ReSharper restore ReturnTypeCanBeEnumerable.Global
    }
}