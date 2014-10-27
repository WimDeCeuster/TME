using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.LegacyAdapter.Equipment
{
    public class OptionInfo : IOptionInfo
    {
        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.CarOption Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public OptionInfo(TMME.CarConfigurator.CarOption adaptee)
        {
            Adaptee = adaptee;
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
    }
}