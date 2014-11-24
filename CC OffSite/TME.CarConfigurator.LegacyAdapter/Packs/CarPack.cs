using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.LegacyAdapter.Colours;
using TME.CarConfigurator.LegacyAdapter.Equipment;
using TME.CarConfigurator.LegacyAdapter.Extensions;
using Legacy = TMME.CarConfigurator;
using IPrice = TME.CarConfigurator.Interfaces.Core.IPrice;

namespace TME.CarConfigurator.LegacyAdapter.Packs
{
    public class CarPack : BaseObject, ICarPack
    {
        
        #region Dependencies (Adaptee)
        private Legacy.CarPack Adaptee
        {
            get;
            set;
        }
        private Legacy.Car CarOfAdaptee
        {
            get; 
            set;
        }
        #endregion

        #region Constructor
        public CarPack(Legacy.CarPack adaptee, Legacy.Car carOfAdaptee)
            : base(adaptee)
        {
            Adaptee = adaptee;
            CarOfAdaptee = carOfAdaptee;
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
                if (Adaptee.AvailableForExteriorColours == null) return new List<IExteriorColourInfo>();
                return
                    Adaptee.AvailableForExteriorColours.Cast<Legacy.CarExteriorColour>()
                        .Select(x => new ExteriorColourInfo(x))
                        .ToList();
            }
        }

        public IReadOnlyList<IUpholsteryInfo> AvailableForUpholsteries
        {
            get
            {
                if (Adaptee.AvailableForUpholsteries == null) return new List<IUpholsteryInfo>();
                return
                    Adaptee.AvailableForUpholsteries.Cast<Legacy.CarUpholstery>()
                        .Select(x => new UpholsteryInfo(x))
                        .ToList();
            }
        }

        public IReadOnlyList<IAccentColourCombination> AccentColourCombinations
        {
            get
            {
                return Adaptee.AccentColourCombinations.Select(x => new AccentColourCombination(x)).ToList();
            }
        }

        public ICarPackEquipment Equipment
        {
            get
            {
                return new CarPackEquipment(Adaptee, CarOfAdaptee);
            }
        }
    }


}