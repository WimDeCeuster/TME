using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.LegacyAdapter.Extensions;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class SubModel : BaseObject, ISubModel
    {
        #region Dependencies (Adaptee)
        private Legacy.SubModel Adaptee { get; set; }
        #endregion

        #region Constructor
        public SubModel(Legacy.SubModel adaptee) 
            : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public IGeneration Generation
        {
            get { return new Generation(Adaptee.Generation) ; }
        }

        public IEnumerable<IVisibleInModeAndView> VisibleIn
        {
            get { return Adaptee.Assets.GetVisibleInModeAndViews(); }
        }

        public IEnumerable<IAsset> Assets
        {
            get { return Adaptee.Assets.GetPlainAssets(); }
        }
    }
}