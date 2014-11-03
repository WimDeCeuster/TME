using System.Collections.Generic;

namespace TME.CarConfigurator.Repository.Objects.Interfaces
{
    public interface IAvailabilityProperties
    {
        IReadOnlyList<CarInfo> StandardOn { get; }
        IReadOnlyList<CarInfo> OptionalOn { get; }
        IReadOnlyList<CarInfo> NotAvailableOn { get; }
    }
}