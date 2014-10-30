using System.Collections.Generic;
using TME.CarConfigurator.Equipment;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator
{
    public class SubModelGradeEquipment : GradeEquipment
    {
        public SubModelGradeEquipment(IEnumerable<IGradeAccessory> accessories, IEnumerable<IGradeOption> options) 
            : base(accessories, options)
        {
        }
    }
}