using System.Collections.Generic;

namespace TME.CarConfigurator.Interfaces.Rules
{
    public interface IRuleSet
    {
        IReadOnlyList<IEquipmentRule> Options { get; }
        IReadOnlyList<IEquipmentRule> Accessories { get; }
        IReadOnlyList<IPackRule> Packs { get; }
    }
}
