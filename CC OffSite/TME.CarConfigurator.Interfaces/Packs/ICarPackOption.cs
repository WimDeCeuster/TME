using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.Interfaces.Packs
{
    public interface ICarPackOption : ICarPackEquipmentItem, IOption
    {
        bool PostProductionOption { get; }
        bool SuffixOption { get; }
    }
}