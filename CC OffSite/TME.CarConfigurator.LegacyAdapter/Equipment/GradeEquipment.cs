using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces.Equipment;
using Legacy = TMME.CarConfigurator;


namespace TME.CarConfigurator.LegacyAdapter.Equipment
{
    public class GradeEquipment :  IGradeEquipment
    {

        #region Dependencies (Adaptee)
        private Legacy.Grade Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public GradeEquipment(Legacy.Grade adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public IReadOnlyList<IGradeAccessory> Accessories
        {
            get
            {
                return Adaptee.Equipment
                        .Cast<Legacy.EquipmentCompareItem>()
                        .Where(x => x.Type == Legacy.EquipmentType.Accessory)
                        .Select(x => new GradeAccesory((Legacy.EquipmentCompareAccessory)x))
                        .ToList();
            }
        }

        public IReadOnlyList<IGradeOption> Options
        {
            get
            {
                return Adaptee.Equipment
                        .Cast<Legacy.EquipmentCompareItem>()
                        .Where(x => x.Type != Legacy.EquipmentType.Accessory)
                        .Select(x => new GradeOption((Legacy.EquipmentCompareOption)x))
                        .ToList();
            }
        }
    }
}
