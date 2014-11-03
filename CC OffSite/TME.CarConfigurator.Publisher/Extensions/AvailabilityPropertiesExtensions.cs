using TME.CarConfigurator.Repository.Objects.Interfaces;

namespace TME.CarConfigurator.Publisher.Extensions
{
    public static class AvailabilityPropertiesExtensions
    {
        public static bool CalculateStandard(this IAvailabilityProperties mappedEquipmentItem)
        {
            return mappedEquipmentItem.StandardOn.Count >= mappedEquipmentItem.OptionalOn.Count && mappedEquipmentItem.StandardOn.Count >= mappedEquipmentItem.NotAvailableOn.Count;
        }

        public static bool CalculateOptional(this IAvailabilityProperties mappedEquipmentItem)
        {
            return mappedEquipmentItem.OptionalOn.Count > mappedEquipmentItem.StandardOn.Count && mappedEquipmentItem.OptionalOn.Count >= mappedEquipmentItem.NotAvailableOn.Count;
        }

        public static bool CalculateNotAvailable(this IAvailabilityProperties mappedEquipmentItem)
        {
            return mappedEquipmentItem.NotAvailableOn.Count > mappedEquipmentItem.StandardOn.Count && mappedEquipmentItem.NotAvailableOn.Count > mappedEquipmentItem.OptionalOn.Count;
        } 
    }
}