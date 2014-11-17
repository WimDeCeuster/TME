using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.LegacyAdapter.Extensions;

namespace TME.CarConfigurator.LegacyAdapter.Colours
{
    public class ColourCombination : IColourCombination
    {
        
        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.CarColourCombination  Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor

        public ColourCombination(TMME.CarConfigurator.CarColourCombination adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion


        public Guid ID
        {
            get { return Adaptee.ID; }
        }

        public IExteriorColour ExteriorColour
        {
            get { return new CarExteriorColour(Adaptee.ExteriorColour); }
        }

        public IUpholstery Upholstery
        {
            get { return new Upholstery(Adaptee.Upholstery);}
        }

        public int SortIndex
        {
            get { return Adaptee.SortIndex; }
        }

        private IReadOnlyList<IVisibleInModeAndView> _visibleIn = null;
        public IReadOnlyList<IVisibleInModeAndView> VisibleIn
        {
            get { return _visibleIn ?? (_visibleIn = Adaptee.Assets.GetVisibleInModeAndViews()); }
        }

        private IReadOnlyList<IAsset> _assets = null;
        public IReadOnlyList<IAsset> Assets
        {
            get { return _assets ?? (_assets = Adaptee.Assets.GetPlainAssets()); }
        }
    }
}
