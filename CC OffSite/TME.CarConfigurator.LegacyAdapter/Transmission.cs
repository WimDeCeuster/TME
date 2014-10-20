using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.LegacyAdapter.Extensions;
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
                  return Adaptee.Assets.GetVisibleInModeAndViews();
            }
        }

        public IEnumerable<IAsset> Assets
        {
            get
            {
                return Adaptee.Assets.GetPlainAssets();
            }


        }
    }
}
