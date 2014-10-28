using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator
{
    public class GradeAccessory : GradeEquipmentItem, IGradeAccessory
    {
        public GradeAccessory(Repository.Objects.Equipment.GradeAccessory repoAccessory)
            : base(repoAccessory)
        {

        }
    }
}
