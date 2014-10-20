namespace TME.CarConfigurator.Interfaces.Equipment
{
    public interface IOption : IEquipmentItem
    {
        bool TechnologyItem { get; }
        IOptionInfo ParentOption { get; }
    }
}