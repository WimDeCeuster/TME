using TME.CarConfigurator.Interfaces;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class CarInfo : ICarInfo
    {

        #region Dependencies (Adaptee)
        private Legacy.Car Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public CarInfo(Legacy.Car adaptee)
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
