using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.S3.Publisher.Extensions
{
    public static class EquipmentItemExtensions
    {
        public static IOrderedEnumerable<T> OrderEquipment<T>(this IEnumerable<T> equipmentItems)
            where T : EquipmentItem
        {
            return equipmentItems.OrderBy(equipmentItem => equipmentItem.Category.SortIndex)
                                 .ThenBy(equipmentItem => equipmentItem.SortIndex)
                                 .ThenBy(equipmentItem => equipmentItem.Name)
                                 .ThenBy(equipmentItem => equipmentItem.InternalCode);
        }
    }
}
