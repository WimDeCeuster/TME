using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Interfaces;

namespace TME.CarConfigurator.Repository.Objects.Equipment
{
    public abstract class GradeEquipmentItem : EquipmentItem, IAvailabilityProperties
    {
        public bool Standard { get; set; }
        public bool Optional { get; set; }
        public bool NotAvailable { get; set; }

        public IReadOnlyList<CarInfo> StandardOn { get; set; }
        public IReadOnlyList<CarInfo> OptionalOn { get; set; }
        public IReadOnlyList<CarInfo> NotAvailableOn { get; set; }

    }
}
