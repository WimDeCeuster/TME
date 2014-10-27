using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.LegacyAdapter.Extensions;

namespace TME.CarConfigurator.LegacyAdapter.Packs
{
    public class GradePack : BaseObject, IGradePack
    {
        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.PackCompareItem Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor

        public GradePack(TMME.CarConfigurator.PackCompareItem adaptee)
            : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        private TMME.CarConfigurator.Car GetCar()
        {
            if (Adaptee.StandardOn.Count > 0) return Adaptee.StandardOn[0];
            if (Adaptee.OptionalOn.Count > 0) return Adaptee.OptionalOn[0];
            return Adaptee.NotAvailableOn[0];
        }
        private TMME.CarConfigurator.CarPack GetCarPack()
        {
            return GetCar().Packs[Adaptee.ID];
        }

        public int ShortID
        {
            get { return GetCarPack().ShortID; }
        }

        public bool GradeFeature
        {
            get { return Adaptee.GradeFeature; }
        }

        public bool OptionalGradeFeature
        {
            get { return Adaptee.OptionalGradeFeature; }
        }

        public IEnumerable<IAsset> Assets
        {
            get { return GetCarPack().Assets.GetPlainAssets(); }
        }

        public bool Standard
        {
            get { return Adaptee.Standard; }
        }

        public bool Optional
        {
            get { return Adaptee.Optional; }
        }

        public bool NotAvailable
        {
            get { return Adaptee.NotAvailable; }
        }

        public IEnumerable<ICarInfo> StandardOn
        {
            get { return Adaptee.StandardOn.Cast<TMME.CarConfigurator.Car>().Select(x => new CarInfo(x)); }
        }

        public IEnumerable<ICarInfo> OptionalOn
        {
            get { return Adaptee.OptionalOn.Cast<TMME.CarConfigurator.Car>().Select(x => new CarInfo(x)); }
        }

        public IEnumerable<ICarInfo> NotAvailableOn
        {
            get
            {
                return Adaptee.NotAvailableOn.Cast<TMME.CarConfigurator.Car>().Select(x => new CarInfo(x));
            }
        }
    }
}
