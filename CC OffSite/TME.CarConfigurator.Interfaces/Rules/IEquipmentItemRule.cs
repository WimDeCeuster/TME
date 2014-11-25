
using TME.CarConfigurator.Interfaces.Enums;

namespace TME.CarConfigurator.Interfaces.Rules
{
    public interface IEquipmentRule : IRule
    {
        ColouringModes ColouringModes { get; }
    }
}