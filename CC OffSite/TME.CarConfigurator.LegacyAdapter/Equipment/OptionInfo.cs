using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.LegacyAdapter.Equipment
{
    public class OptionInfo : IOptionInfo
    {
  
        #region Constructor
        public OptionInfo(TMME.CarConfigurator.CarOption adaptee)
        {
            ShortID = adaptee.ShortID;
            Name = adaptee.Name;
        }
        public OptionInfo(TMME.CarConfigurator.CarPackOption adaptee)
        {
            ShortID = adaptee.ShortID;
            Name = adaptee.Name;
        }
        #endregion

        public int ShortID { get; private set; }
        public string Name { get; private set; }
    }
}