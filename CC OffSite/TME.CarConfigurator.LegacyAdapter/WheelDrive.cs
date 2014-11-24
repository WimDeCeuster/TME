using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.LegacyAdapter.Extensions;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class WheelDrive : BaseObject, IWheelDrive
    {
        #region Dependencies (Adaptee)
        private Legacy.WheelDrive Adaptee
        {
            get;
            set;
        }
        private bool ForCar
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public WheelDrive(Legacy.WheelDrive adaptee, bool forCar)
            : base(adaptee)
        {
            Adaptee = adaptee;
            ForCar = forCar;
        }
        #endregion

        public bool KeyFeature
        {
            get { return Adaptee.KeyFeature; }
        }

        public bool Brochure
        {
            get { return Adaptee.Brochure; }
        }

        private IReadOnlyList<IVisibleInModeAndView> _visibleIn = null;
        public IReadOnlyList<IVisibleInModeAndView> VisibleIn
        {
            get { return _visibleIn ?? (_visibleIn = (ForCar ? Adaptee.Assets.GetVisibleInModeAndViews() : Adaptee.Assets.GetVisibleInModeAndViewsWithoutAssets())); }
        }

        private IReadOnlyList<IAsset> _assets = null;
        public IReadOnlyList<IAsset> Assets
        {
            get { return _assets ?? (_assets = Adaptee.Assets.GetPlainAssets()); }
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
