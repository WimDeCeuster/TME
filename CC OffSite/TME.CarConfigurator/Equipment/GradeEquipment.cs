using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.Equipment
{
    public class GradeEquipment : IGradeEquipment
    {
        public IReadOnlyList<IGradeAccessory> GradeAccessories { get; private set; }
        public IReadOnlyList<IGradeOption> GradeOptions { get; private set; }

        public GradeEquipment(IEnumerable<IGradeAccessory> gradeAccessories, IEnumerable<IGradeOption> gradeOptions)
        {
            if (gradeAccessories == null) throw new ArgumentNullException("gradeAccessories");
            if (gradeOptions == null) throw new ArgumentNullException("gradeOptions");

            GradeAccessories = gradeAccessories.ToList();
            GradeOptions = gradeOptions.ToList();
        }
    }
}
