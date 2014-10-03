using TME.CarConfigurator.Interfaces;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class CarConfiguratorVersion : ICarConfiguratorVersion
    {

                
        #region Dependencies (Adaptee)
        private Legacy.CarConfiguratorVersionInfo Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public CarConfiguratorVersion(Legacy.CarConfiguratorVersionInfo adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion


        public short ID
        {
            get { return Adaptee.ID; }
        }

        public string Name
        {
            get { return Adaptee.Name; }
        }
    }
}
