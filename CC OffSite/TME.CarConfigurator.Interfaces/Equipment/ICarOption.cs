namespace TME.CarConfigurator.Interfaces.Equipment
{
    public interface ICarOption : ICarEquipmentItem, IOption
    {
        bool PostProductionOption { get; }
        bool SuffixOption { get; }
    }
}