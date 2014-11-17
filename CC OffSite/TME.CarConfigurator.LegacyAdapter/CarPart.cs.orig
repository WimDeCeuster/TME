using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.LegacyAdapter.Extensions;
using Legacy = TMME.CarConfigurator;


namespace TME.CarConfigurator.LegacyAdapter
{
    public class CarPart : ICarPart
    {

        #region Dependencies (Adaptee)
        private Legacy.CarPart Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public CarPart(Legacy.CarPart adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public string Code
        {
            get { return Adaptee.Code; }
        }

        public string Name
        {
            get { return Adaptee.Name; }
        }

<<<<<<< HEAD
        private IReadOnlyList<IVisibleInModeAndView> _visibleIn = null;
=======
        public Guid ID { get { return Adaptee.ID; } }

>>>>>>> b5b6c1432c771b5adc8add0d876e3d0f7f291acb
        public IReadOnlyList<IVisibleInModeAndView> VisibleIn
        {
            get { return _visibleIn ?? (_visibleIn = Adaptee.Assets.GetVisibleInModeAndViews()); }
        }

    }
}
