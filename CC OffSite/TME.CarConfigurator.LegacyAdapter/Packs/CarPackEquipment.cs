using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.Packs;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter.Packs
{

    public class CarPackEquipment : ICarPackEquipment
    {

       #region Dependencies (Adaptee)
        private Legacy.CarPack Adaptee
        {
            get;
            set;
        }

        private Legacy.Car CarOfAdaptee
        {
            get; 
            set;
        }
        #endregion

        #region Constructor
        public CarPackEquipment(Legacy.CarPack adaptee, Legacy.Car carOfAdaptee)
        {
            Adaptee = adaptee;
            CarOfAdaptee = carOfAdaptee;
        }


        #endregion

        public IReadOnlyList<ICarPackAccessory> Accessories
        {
            get
            {
                return
                    Adaptee.Equipment.OfType<Legacy.CarPackAccessory>()
                        .Select(x => new CarPackAccessory(x, CarOfAdaptee.Equipment.OfType<Legacy.CarAccessory>().FirstOrDefault(y=>y.ID == x.ID)))
                        .ToList();
            }
        }

        public IReadOnlyList<ICarPackOption> Options
        {
            get
            {
                return
                    Adaptee.Equipment.OfType<Legacy.CarPackOption>()
                        .Select(x => new CarPackOption(x, CarOfAdaptee.Equipment.OfType<Legacy.CarOption>().FirstOrDefault(y => y.ID == x.ID)))
                        .ToList();
            }
        }

        public IReadOnlyList<ICarPackExteriorColourType> ExteriorColourTypes
        {
            get
            {
                return
                    Adaptee.Equipment.OfType<Legacy.CarPackExteriorColourType>()
                        .Select(x => new CarPackExteriorColourType(x, CarOfAdaptee.Equipment.OfType<Legacy.CarExteriorColourType>().FirstOrDefault(y => y.ID == x.ID)))
                        .ToList();
            }
        }

        public IReadOnlyList<ICarPackUpholsteryType> UpholsteryTypes
        {
            get
            {
                return
                    Adaptee.Equipment.OfType<Legacy.CarPackUpholsteryType>()
                        .Select(x => new CarPackUpholsteryType(x, CarOfAdaptee.Equipment.OfType<Legacy.CarUpholsteryType>().FirstOrDefault(y => y.ID == x.ID)))
                        .ToList();
            }
        }
    }
}