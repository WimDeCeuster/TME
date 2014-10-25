using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces.Equipment;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class BestVisibleIn : IBestVisibleIn
    {

        #region Dependencies (Adaptee)
        private Legacy.BestVisibleIn Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public BestVisibleIn(Legacy.BestVisibleIn adaptee)
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
