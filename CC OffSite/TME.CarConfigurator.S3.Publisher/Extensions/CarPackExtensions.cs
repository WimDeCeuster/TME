using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Repository.Objects.Packs;

namespace TME.CarConfigurator.S3.Publisher.Extensions
{
    public static class CarPackExtensions
    {
        public static IEnumerable<CarPack> OrderEquipment(this IEnumerable<CarPack> carPacks)
        {
            foreach (var carPack in carPacks)
            {
                carPack.Equipment = new CarPackEquipment
                {
                    Accessories = carPack.Equipment.Accessories.OrderEquipment().ToList(),
                    Options = carPack.Equipment.Options.OrderEquipment().ToList(),
                    ExteriorColourTypes = carPack.Equipment.ExteriorColourTypes.OrderEquipment().ToList(),
                    UpholsteryTypes = carPack.Equipment.UpholsteryTypes.OrderEquipment().ToList()
                };
            }

            return carPacks;
        }
    }
}