using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.Interfaces.Packs
{
    interface ICarPackEquipmentItem : ICarEquipmentItem
    {
    }

    interface ICarPackOption : ICarPackEquipmentItem, ICarOption
    {
    }

    interface ICarPackAccessory : ICarPackEquipmentItem, ICarAccessory
    {

    }
}
