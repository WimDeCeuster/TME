using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.Repository.Objects.Interfaces;

namespace TME.CarConfigurator.Repository.Objects.Equipment
{
    public class CarEquipmentItem : EquipmentItem , IAvailabilityProperties
    {
        public bool Standard { get; set; }
        public bool Optional { get; set; }

        public List<ExteriorColourInfo> AvailableForExteriorColours { get; set; }
        public List<UpholsteryInfo> AvailableForUpholsteries { get; set; }

        public List<VisibleInModeAndView> VisibleIn { get; set; }

        public IReadOnlyList<CarInfo> StandardOn { get; set; }
        public IReadOnlyList<CarInfo> OptionalOn { get; set; }
        public IReadOnlyList<CarInfo> NotAvailableOn { get; set; }
    }
}
