using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.LegacyAdapter.Extensions;

namespace TME.CarConfigurator.LegacyAdapter.Colours
{
    public class CarColourCombination : ICarColourCombination
    {
        
        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.CarColourCombination  Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor

        public CarColourCombination(TMME.CarConfigurator.CarColourCombination adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion


        public Guid ID
        {
            get { return Adaptee.ID; }
        }

        public ICarExteriorColour ExteriorColour
        {
            get { return new CarExteriorColour(Adaptee.ExteriorColour, true); }
        }

        public ICarUpholstery Upholstery
        {
            get { return new CarUpholstery(Adaptee.Upholstery, true); }
        }

        public int SortIndex
        {
            get { return Adaptee.SortIndex; }
        }

        private IReadOnlyList<IVisibleInModeAndView> _visibleIn;
        public IReadOnlyList<IVisibleInModeAndView> VisibleIn
        {
            get
            {
                if (_visibleIn != null) return _visibleIn;

                _visibleIn =  Adaptee.Assets.GetVisibleInModeAndViews();

                return _visibleIn;
            }
        }

        private IReadOnlyList<IAsset> _assets;
        public IReadOnlyList<IAsset> Assets
        {
            get { return _assets ?? (_assets = Adaptee.Assets.GetPlainAssets()); }
        }
    }
}
