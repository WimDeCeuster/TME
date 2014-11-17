using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.LegacyAdapter.Extensions;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class BodyType : BaseObject, IBodyType
    {
        #region Dependencies (Adaptee)
        private Legacy.BodyType Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public BodyType(Legacy.BodyType adaptee) : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        
        public int NumberOfDoors
        {
            get { return Adaptee.NumberOfDoors; }
        }

        public int NumberOfSeats
        {
            get { return Adaptee.NumberOfSeats; }
        }


        private IReadOnlyList<IVisibleInModeAndView> _visibleIn = null;
        public IReadOnlyList<IVisibleInModeAndView> VisibleIn
        {
            get { return _visibleIn ?? (_visibleIn = Adaptee.Assets.GetVisibleInModeAndViews()); }
        }

        private IReadOnlyList<IAsset> _assets = null;
        public IReadOnlyList<IAsset> Assets
        {
            get { return _assets ?? (_assets =Adaptee.Assets.GetPlainAssets()); }
        }


        public bool VisibleInExteriorSpin
        {
            get { return Adaptee.VisibleInExteriorSpin; }
        }

        public bool VisibleInInteriorSpin
        {
            get { return Adaptee.VisibleInInteriorSpin; }
        }

        public bool VisibleInXRay4X4Spin
        {
            get { return Adaptee.VisibleInXRay4x4Spin; }
        }

        public bool VisibleInXRayHybridSpin
        {
            get { return Adaptee.VisibleInXRayHybridSpin; }
        }

        public bool VisibleInXRaySafetySpin
        {
            get { return Adaptee.VisibleInXRaySafetySpin; }
        }

 
    }
}
