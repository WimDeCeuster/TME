using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.LegacyAdapter.Equipment
{
    public class BestVisibleIn : IBestVisibleIn
    {

        #region Constructor
        public BestVisibleIn(TMME.CarConfigurator.BestVisibleIn adaptee)
        {
            Mode = adaptee.Mode;
            View = adaptee.View;
            Angle = adaptee.Angle;
        }
        public BestVisibleIn()
        {
            Mode = string.Empty;
            View = string.Empty;
            Angle = 0;
        }
        #endregion

        public string Mode { get; private set; }
        public string View { get; private set; }
        public int Angle { get; private set; }
    }
}
