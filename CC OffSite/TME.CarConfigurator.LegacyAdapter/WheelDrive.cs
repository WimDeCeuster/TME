using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
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
        #endregion

        #region Constructor
        public WheelDrive(Legacy.WheelDrive adaptee)
            : base(adaptee)
        {
            Adaptee = adaptee;
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

        public IEnumerable<IVisibleInModeAndView> VisibleIn
        {
            get
            {

                var groups = Adaptee.Assets.Cast<Legacy.Asset>()
                    .Where(x => x.AssetType.Mode.Length == 0)
                    .GroupBy(x => new { x.AssetType.Details.Mode, x.AssetType.Details.View })
                    .Select(group => new VisibleInModeAndView()
                    {
                        Mode = group.Key.Mode,
                        View = group.Key.View,
                        Assets = group.Select(legacyAsset => new Asset(legacyAsset))

                    });

                return groups;
            }
        }

        public IEnumerable<IAsset> Assets
        {
            get
            {
                return Adaptee.Assets.Cast<Legacy.Asset>()
                    .Where(x => x.AssetType.Mode.Length == 0)
                    .Select(x => new Asset(x));
            }
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
