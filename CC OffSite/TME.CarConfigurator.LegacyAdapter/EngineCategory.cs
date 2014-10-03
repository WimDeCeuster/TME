using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class EngineCategory : BaseObject, IEngineCategory
    {
        #region Dependencies (Adaptee)
        private Legacy.EngineCategory Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public EngineCategory(Legacy.EngineCategory adaptee)
            : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public IEnumerable<IAsset> Assets
        {
            get { return Adaptee.Assets.Cast<Legacy.Asset>().Select(x => new Asset(x)); }     
        }
    }
}
