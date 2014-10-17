using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class Transmission : BaseObject, ITransmission
    {
        #region Dependencies (Adaptee)
        private Legacy.Transmission Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public Transmission(Legacy.Transmission adaptee)
            : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion
        

        public ITransmissionType Type
        {
            get { return new TransmissionType(Adaptee.Type);}
        }

        public bool KeyFeature
        {
            get { return Adaptee.KeyFeature; }
        }

        public bool Brochure
        {
            get { return Adaptee.Brochure; }
        }

        public int NumberOfGears
        {
            get { return Adaptee.NumberOfGears; }
        }

        public IEnumerable<IVisibleInModeAndView> VisibleIn
        {
            get
            {
                var groups = Adaptee.Assets.Cast<Legacy.Asset>()
                    .Where(a => a.AssetType.Mode.Length == 0)
                    .GroupBy(a => new {a.AssetType.Details.Mode, a.AssetType.Details.View})
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
            get {
                return Adaptee.Assets.Cast<Legacy.Asset>()
                    .Where(x => x.AssetType.Mode.Length == 0)
                    .Select(x => new Asset(x)); }
        }


        //TODO Wim, Put Properties In Legacy.Transmission.
        public bool VisibleInExteriorSpin
        {
            get { return false; }
        }

        public bool VisibleInInteriorSpin
        {
            get { return false; }
        }

        public bool VisibleInXRay4X4Spin
        {
            get { return false; }
        }

        public bool VisibleInXRayHybridSpin
        {
            get { return false; }
        }

        public bool VisibleInXRaySafetySpin
        {
            get { return false; }
        }
    }
}
