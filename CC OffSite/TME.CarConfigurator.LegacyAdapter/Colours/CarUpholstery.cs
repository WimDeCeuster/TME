using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.LegacyAdapter.Extensions;

namespace TME.CarConfigurator.LegacyAdapter.Colours
{
    public class CarUpholstery : BaseObject, ICarUpholstery
    {
        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.CarUpholstery Adaptee
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
        public CarUpholstery(TMME.CarConfigurator.CarUpholstery adaptee, bool forCar)
            : base(adaptee)
        {
            Adaptee = adaptee;
            ForCar = forCar;
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

        public Interfaces.Core.IPrice Price
        {
            get { return new Price(Adaptee); }
        }
    }
}
