using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;
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

        public IEnumerable<IVisibleInModeAndView> VisibleIn { get{throw new NotImplementedException();} }

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

        public IEnumerable<IAsset> Assets
        {
            get { return Adaptee.Assets.Cast<Legacy.Asset>().Select(x => new Asset(x)); }
        }
    }
}
