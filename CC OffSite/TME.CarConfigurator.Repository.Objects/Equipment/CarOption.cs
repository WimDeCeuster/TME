using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects.Equipment
{
    public class CarOption : CarEquipmentItem
    {

        public Price Price { get; set; }

        public bool TechnologyItem { get; set; }
        public int ParentOptionShortID { get; set; }
    }
}
