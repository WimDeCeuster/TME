using TME.CarConfigurator.Interfaces.Enums;

namespace TME.CarConfigurator.Interfaces.Rules
{
    public interface IRule
    {
        int ShortID { get; }
        string Name { get; }
        RuleCategory Category { get; }
    }
}
