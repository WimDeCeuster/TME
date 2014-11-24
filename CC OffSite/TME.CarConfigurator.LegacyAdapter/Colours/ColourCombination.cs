using System;
using System.Collections.Generic;
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
        private bool ForCar
        {
            get;
            set;
        }
        #endregion

        #region Constructor

        public ColourCombination(TMME.CarConfigurator.CarColourCombination adaptee, bool forCar)
        {
            Adaptee = adaptee;
            ForCar = forCar;
        }
        #endregion


        public Guid ID
        {
            get { return Adaptee.ID; }
        }

        public IExteriorColour ExteriorColour
        {
            get { return new CarExteriorColour(Adaptee.ExteriorColour, ForCar); }
        }

        public IUpholstery Upholstery
        {
            get { return new CarUpholstery(Adaptee.Upholstery, ForCar);}
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

                _visibleIn = ForCar 
                    ? Adaptee.Assets.GetVisibleInModeAndViews() 
                    : Adaptee.Assets.GetVisibleInModeAndViewsWithoutAssets();

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
