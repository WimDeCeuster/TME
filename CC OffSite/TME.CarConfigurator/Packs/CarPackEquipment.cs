using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.Packs;

namespace TME.CarConfigurator.Packs
{
    public class CarPackEquipment : ICarPackEquipment
    {
        public CarPackEquipment(IEnumerable<ICarPackAccessory> accessories, IEnumerable<ICarPackOption> options, IEnumerable<ICarPackExteriorColourType> exteriorColourTypes, IEnumerable<ICarPackUpholsteryType> upholsteryTypes)
        {
            if (accessories == null) throw new ArgumentNullException("accessories");
            if (options == null) throw new ArgumentNullException("options");
            if (exteriorColourTypes == null) throw new ArgumentNullException("exteriorColourTypes");
            if (upholsteryTypes == null) throw new ArgumentNullException("upholsteryTypes");

            Options = options.ToList();
            Accessories = accessories.ToList();
            ExteriorColourTypes = exteriorColourTypes.ToList();
            UpholsteryTypes = upholsteryTypes.ToList();
        }

        public IReadOnlyList<ICarPackAccessory> Accessories { get; private set; }

        public IReadOnlyList<ICarPackOption> Options { get; private set; }

        public IReadOnlyList<ICarPackExteriorColourType> ExteriorColourTypes { get; private set; }

        public IReadOnlyList<ICarPackUpholsteryType> UpholsteryTypes { get; private set; }
    }
}
