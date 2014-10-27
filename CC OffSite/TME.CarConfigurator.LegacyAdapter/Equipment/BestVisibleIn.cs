using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.LegacyAdapter.Equipment
{
    public class BestVisibleIn : IBestVisibleIn
    {

        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.BestVisibleIn Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public BestVisibleIn(TMME.CarConfigurator.BestVisibleIn adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion


        public string Mode
        {
            get { return Adaptee.Mode; }
        }

        public string View
        {
            get { return Adaptee.View; }
        }

        public int Angle
        {
            get { return Adaptee.Angle; }
        }
    }
}
