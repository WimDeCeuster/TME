using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Colours;

namespace TME.CarConfigurator.Repository.Objects.Equipment
{
    public class CarEquipmentItem : EquipmentItem
    {
        public bool Standard { get; set; }
        public bool Optional { get; set; }

        public List<ExteriorColourInfo> AvailableForExteriorColours { get; set; }
        List<UpholsteryInfo> AvailableForUpholsteries { get; set; }

        public List<VisibleInModeAndView> VisibleIn { get; set; }
    }
}
