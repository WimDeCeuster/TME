using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.LegacyAdapter.Extensions;

namespace TME.CarConfigurator.LegacyAdapter.Colours
{
    public class Upholstery : BaseObject, IUphostery
    {
        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.CarUpholstery Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public Upholstery(TMME.CarConfigurator.CarUpholstery adaptee)
            : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public string InteriorColourCode
        {
            get { return Adaptee.InteriorColourCode; }
        }

        public string TrimCode
        {
            get { return Adaptee.TrimCode; }
        }

        public IUpholsteryType Type
        {
            get { return new UpholsteryType(Adaptee.Type); }
        }

        public IEnumerable<IAsset> Assets
        {
            get { return Adaptee.Assets.GetPlainAssets(); }
        }
    }
}
