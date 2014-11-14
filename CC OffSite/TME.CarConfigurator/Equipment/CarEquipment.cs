using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.Equipment
{
    public class CarEquipment : ICarEquipment
    {
        public virtual IReadOnlyList<ICarAccessory> Accessories { get; private set; }
        public virtual IReadOnlyList<ICarOption> Options { get; private set; }

        public CarEquipment(IEnumerable<ICarAccessory> accessories, IEnumerable<ICarOption> options)
        {
            if (accessories == null) throw new ArgumentNullException("accessories");
            if (options == null) throw new ArgumentNullException("options");

            Options = options.ToList();
            Accessories = accessories.ToList();
        }
    }
}