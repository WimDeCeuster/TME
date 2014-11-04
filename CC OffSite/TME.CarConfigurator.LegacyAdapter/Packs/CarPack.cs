using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.LegacyAdapter.Colours;
using TME.CarConfigurator.LegacyAdapter.Extensions;
using IPrice = TME.CarConfigurator.Interfaces.Core.IPrice;

namespace TME.CarConfigurator.LegacyAdapter.Packs
{
    public class CarPack : BaseObject, ICarPack
    {
        
        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.CarPack Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public CarPack(TMME.CarConfigurator.CarPack adaptee)
            : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public int ShortID
        {
            get { return Adaptee.ShortID; }
        }

        public bool GradeFeature
        {
            get { return Adaptee.GradeFeature; }
        }

        public bool OptionalGradeFeature
        {
            get { return Adaptee.OptionalGradeFeature; }
        }

        public bool Standard
        {
            get { return Adaptee.Standard; }
        }

        public bool Optional
        {
            get { return Adaptee.Optional; }
        }

        public IPrice Price
        {
            get { return new Price(Adaptee); }
        }

        public IReadOnlyList<IAsset> Assets
        {
            get { return Adaptee.Assets.GetPlainAssets(); }
        }

        public IReadOnlyList<IExteriorColourInfo> AvailableForExteriorColours
        {
            get
            {
                if (Adaptee.AvailableForExteriorColours == null) return null;
                return
                    Adaptee.AvailableForExteriorColours.Cast<TMME.CarConfigurator.CarExteriorColour>()
                        .Select(x => new ExteriorColourInfo(x))
                        .ToList();
            }
        }

        public IReadOnlyList<IUpholsteryInfo> AvailableForUpholsteries
        {
            get
            {
                if (Adaptee.AvailableForUpholsteries == null) return null;
                return
                    Adaptee.AvailableForUpholsteries.Cast<TMME.CarConfigurator.CarUpholstery>()
                        .Select(x => new UpholsteryInfo(x))
                        .ToList();
            }
        }
    }
}