using TME.CarConfigurator.Interfaces.Core;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class Price : IPrice
    {


        #region Constructor
        public Price(Legacy.IPrice adaptee)
        {
            PriceInVat = adaptee.PriceInVat;
            PriceExVat = adaptee.PriceExVat;
        }

        internal Price()
        {
            
        }
        #endregion

        public decimal PriceInVat
        {
            get;
            internal set;
        }

        public decimal PriceExVat
        {
            get;
            internal set;
        }
    }
}
