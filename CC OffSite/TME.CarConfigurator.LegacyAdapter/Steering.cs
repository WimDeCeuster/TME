using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class Steering : BaseObject, ISteering
    {
        #region Dependencies (Adaptee)
        private Legacy.Steering Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public Steering(Legacy.Steering adaptee)
            : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion
    }
}
