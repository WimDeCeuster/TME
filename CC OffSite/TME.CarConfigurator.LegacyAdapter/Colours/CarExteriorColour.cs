using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.LegacyAdapter.Extensions;

namespace TME.CarConfigurator.LegacyAdapter.Colours
{
    public class CarExteriorColour : BaseObject, IExteriorColour
    {
        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.CarExteriorColour Adaptee
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
        public CarExteriorColour(TMME.CarConfigurator.CarExteriorColour adaptee, bool forCar) : base(adaptee)
        {
            Adaptee = adaptee;
            ForCar = forCar;
        }


        #endregion

        public bool Promoted
        {
            get { return Adaptee.IsPromoted; }
        }

        private ColourTransformation _transformation = null;
        public IColourTransformation Transformation
        {
            get
            {
                if (_transformation == null)
                {
                    _transformation = new ColourTransformation(Adaptee.Transformation);
                }

                return _transformation.IsEmpty() ? null : _transformation;
            }
        }

        public IExteriorColourType Type
        {
            get { return new ExteriorColourType(Adaptee.Type); }
        }

        private IReadOnlyList<IVisibleInModeAndView> _visibleIn = null;
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

        private IReadOnlyList<IAsset> _assets = null;
        public IReadOnlyList<IAsset> Assets
        {
            get { return _assets ?? (_assets = Adaptee.Assets.GetPlainAssets()); }
        }
    }
}
