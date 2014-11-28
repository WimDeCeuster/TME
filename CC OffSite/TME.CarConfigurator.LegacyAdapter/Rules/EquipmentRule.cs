using TME.CarConfigurator.Interfaces.Enums;
using TME.CarConfigurator.Interfaces.Rules;
using ColouringModes = TME.CarConfigurator.Interfaces.Enums.ColouringModes;

namespace TME.CarConfigurator.LegacyAdapter.Rules
{
    internal class EquipmentRule : IEquipmentRule
    {
        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.CarEquipmentItem Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor

        public EquipmentRule(TMME.CarConfigurator.CarEquipmentItem carAccessory)
        {
            Adaptee = carAccessory;
        }

        public int ShortID
        {
            get { return Adaptee.ShortID; }
        }
        #endregion

        public string Name
        {
            get { return Adaptee.Name; }
        }

        public ColouringModes ColouringModes
        {
            get { return ColouringModes.None; }
        }

        public RuleCategory Category
        {
            get { return RuleCategory.Visual; }
        }
    }
}