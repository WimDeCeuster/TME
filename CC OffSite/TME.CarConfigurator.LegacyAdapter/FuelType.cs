using TME.CarConfigurator.Interfaces;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class FuelType : BaseObject, IFuelType
    {
        #region Dependencies (Adaptee)
        private Legacy.FuelType Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public FuelType(Legacy.FuelType adaptee)
            : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public bool Hybrid
        {
            get { return Adaptee.Hybrid; }
        }
    }
}
