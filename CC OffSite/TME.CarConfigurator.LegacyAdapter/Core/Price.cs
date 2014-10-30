using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces.Core;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class Price : IPrice
    {

        #region Dependencies (Adaptee)
        private Legacy.IPrice Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public Price(Legacy.IPrice adaptee)
        {
            Adaptee = adaptee;
        }

        #endregion

        public decimal PriceInVat
        {
            get { return Adaptee.PriceInVat; }
        }

        public decimal PriceExVat
        {
            get { return Adaptee.PriceExVat; }
        }
    }
}
