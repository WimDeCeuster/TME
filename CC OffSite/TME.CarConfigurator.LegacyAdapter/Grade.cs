using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class Grade : BaseObject, IGrade
    {
        #region Dependencies (Adaptee)
        private Legacy.Grade Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public Grade(Legacy.Grade adaptee) : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public IGrade BasedUpon
        {
            get { return Adaptee.BasedUpon == null ? null : new Grade(Adaptee.BasedUpon); }
        }

        public IPrice StartingPrice
        {
            get { return new StartingPrice(Adaptee); }
        }

        public string FeatureText
        {
            get { return Adaptee.FeatureText; }
        }

        public string LongDescription
        {
            get { return Adaptee.LongDescription; }
        }

        public bool Special
        {
            get { return Adaptee.Special; }
        }
 
        public IEnumerable<IAsset> Assets
        {
            get { 
                return Adaptee.Assets.Cast<Legacy.Asset>()
                    .Where(x=>x.AssetType.Mode.Length == 0)
                    .Select(x => new Asset(x)); }
        }
    }
}
