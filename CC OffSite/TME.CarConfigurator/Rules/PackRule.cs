using System;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Interfaces.Enums;
using TME.CarConfigurator.Interfaces.Rules;

namespace TME.CarConfigurator.Rules
{
    public class PackRule : IPackRule
    {
        private readonly Repository.Objects.Rules.PackRule _packRule;

        public PackRule(Repository.Objects.Rules.PackRule packRule)
        {
            if (packRule == null) throw new ArgumentNullException("packRule");
            _packRule = packRule;
        }

        public int ShortID { get { return _packRule.ShortID; } }
        public string Name { get { return _packRule.Name; } }
        public RuleCategory Category { get { return _packRule.Category.Convert(); } }
    }
}