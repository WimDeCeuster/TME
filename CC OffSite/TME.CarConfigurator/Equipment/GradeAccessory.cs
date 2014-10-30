using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Factories;

namespace TME.CarConfigurator.Equipment
{
    public class GradeAccessory : GradeEquipmentItem<Repository.Objects.Equipment.GradeAccessory>, IGradeAccessory
    {
        public GradeAccessory(Repository.Objects.Equipment.GradeAccessory repoAccessory, IColourFactory colourFactory)
            : base(repoAccessory, colourFactory)
        {

        }
    }
}
