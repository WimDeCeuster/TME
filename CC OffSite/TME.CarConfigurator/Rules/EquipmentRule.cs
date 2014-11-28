using System;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Interfaces.Enums;
using TME.CarConfigurator.Interfaces.Rules;
using TME.CarConfigurator.Repository.Objects.Rules;

namespace TME.CarConfigurator.Rules
{
    public class EquipmentRule : IEquipmentRule
    {
        private readonly EquipmentItemRule _equipmentItemRule;

        public EquipmentRule(EquipmentItemRule equipmentItemRule)
        {
            if (equipmentItemRule == null) throw new ArgumentNullException("equipmentItemRule");
            _equipmentItemRule = equipmentItemRule;
        }

        public int ShortID { get { return _equipmentItemRule.ShortID; }}
        public string Name { get { return _equipmentItemRule.Name; }}
        public RuleCategory Category { get { return _equipmentItemRule.Category.Convert(); }}
        public ColouringModes ColouringModes { get { return _equipmentItemRule.ColouringModes.Convert(); } }
    }
}