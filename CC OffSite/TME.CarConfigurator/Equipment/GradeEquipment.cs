using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.Equipment
{
    public class GradeEquipment : IGradeEquipment
    {
        public virtual IReadOnlyList<IGradeAccessory> Accessories { get; set; }
        public virtual IReadOnlyList<IGradeOption> Options { get; set; }

        public GradeEquipment(IEnumerable<IGradeAccessory> accessories, IEnumerable<IGradeOption> options)
        {
            if (accessories == null) throw new ArgumentNullException("accessories");
            if (options == null) throw new ArgumentNullException("options");

            Accessories = accessories.ToList();
            Options = options.ToList();
        }
    }
}
