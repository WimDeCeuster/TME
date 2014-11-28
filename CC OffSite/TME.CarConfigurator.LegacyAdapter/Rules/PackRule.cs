using TME.CarConfigurator.Interfaces.Enums;
using TME.CarConfigurator.Interfaces.Rules;

namespace TME.CarConfigurator.LegacyAdapter.Rules
{
    internal class PackRule : IPackRule
    {

        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.CarPack Adaptee
        { 
            get; set;
        }
        #endregion

        #region Constructor
        public PackRule(TMME.CarConfigurator.CarPack carPack)
        {
            Adaptee = carPack;
        }
        #endregion

        public int ShortID
        {
            get { return Adaptee.ShortID; }
        }
        public string Name
        {
            get { return Adaptee.Name; }
        }

        public RuleCategory Category
        {
            get { return RuleCategory.Visual; }
        }
    }
}