using System.Collections.Generic;

namespace TME.CarConfigurator.Repository.Objects.Rules
{
    public class RuleSet
    {
        public RuleSet()
        {
            PackRules = new List<PackRule>();
            OptionRules = new List<EquipmentItemRule>();
            AccessoryRules = new List<EquipmentItemRule>();
        }

        public IList<PackRule> PackRules { get; set; }
        public IList<EquipmentItemRule> OptionRules { get; set; }
        public IList<EquipmentItemRule> AccessoryRules { get; set; }
    }
}