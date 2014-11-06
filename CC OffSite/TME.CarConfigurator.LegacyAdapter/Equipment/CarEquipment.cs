using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.Equipment;
using Legacy = TMME.CarConfigurator;


namespace TME.CarConfigurator.LegacyAdapter.Equipment
{
    public class CarEquipment :  ICarEquipment
    {

        #region Dependencies (Adaptee)
        private Legacy.Car Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public CarEquipment(Legacy.Car adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public IReadOnlyList<ICarAccessory> Accessories
        {
            get
            {
                return Adaptee.Equipment
                        .Cast<Legacy.CarEquipmentItem>()
                        .Where(x => x.Type == Legacy.EquipmentType.Accessory)
                        .Select(x => new CarAccessory((Legacy.CarAccessory)x))
                        .ToList();
            }
        }

        public IReadOnlyList<ICarOption> Options
        {
            get
            {
                return Adaptee.Equipment
                        .Cast<Legacy.CarEquipmentItem>()
                        .Where(x => x.Type != Legacy.EquipmentType.Accessory)
                        .Select(x => new CarOption((Legacy.CarOption)x, Adaptee))
                        .ToList();
            }
        }
    }
}
