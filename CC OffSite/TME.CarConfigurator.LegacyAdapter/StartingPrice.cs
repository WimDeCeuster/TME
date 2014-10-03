using TME.CarConfigurator.Interfaces.Core;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class StartingPrice : IPrice
    {

        #region Dependencies (Adaptee)
        private Legacy.IMinimumPrice Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public StartingPrice(Legacy.IMinimumPrice adaptee)
        {
            Adaptee = adaptee;
        }

        #endregion

        public decimal PriceInVat
        {
            get { return Adaptee.MinimumPriceInVat; }
        }

        public decimal PriceExVat
        {
            get { return Adaptee.MinimumPriceInVat; }
        }
    }
}
