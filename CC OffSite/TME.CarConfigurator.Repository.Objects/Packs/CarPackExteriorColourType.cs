using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Colours;

namespace TME.CarConfigurator.Repository.Objects.Packs
{
    public class CarPackExteriorColourType : CarPackEquipmentItem
    {
        public List<ColourCombinationInfo> ColourCombinations { get; set; }
    }
}