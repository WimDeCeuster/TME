using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Rules;

namespace TME.CarConfigurator.LegacyAdapter.Rules
{
    internal class RuleSet : IRuleSet
    {
        public IReadOnlyList<IEquipmentRule> Options { get; internal set; }
        public IReadOnlyList<IEquipmentRule> Accessories { get; internal set; }
        public IReadOnlyList<IPackRule> Packs { get; internal set; }
    }
}