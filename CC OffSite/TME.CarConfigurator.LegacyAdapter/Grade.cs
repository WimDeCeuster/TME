using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.LegacyAdapter.Equipment;
using TME.CarConfigurator.LegacyAdapter.Extensions;
using TME.CarConfigurator.LegacyAdapter.Packs;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class Grade : BaseObject, IGrade
    {


        #region Dependencies (Adaptee)
        private Legacy.Grade Adaptee
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
        public Grade(Legacy.Grade adaptee, bool forCar)
            : base(adaptee)
        {
            Adaptee = adaptee;
            ForCar = forCar;
        }
        #endregion

        public bool Special
        {
            get { return Adaptee.Special; }
        }

        public IPrice StartingPrice
        {
            get { return new StartingPrice(Adaptee); }
        }

        public IGradeInfo BasedUpon
        {
            get
            {
                return Adaptee.BasedUpon==null ? null : new GradeInfo(Adaptee.BasedUpon);
            }
        }

        private IReadOnlyList<IVisibleInModeAndView> _visibleIn = null;
        public IReadOnlyList<IVisibleInModeAndView> VisibleIn
        {
            get { return _visibleIn ?? (_visibleIn = (ForCar ? Adaptee.Assets.GetVisibleInModeAndViews() : Adaptee.Assets.GetVisibleInModeAndViewsWithoutAssets())); }
        }

        private IReadOnlyList<IAsset> _assets = null;
        public IReadOnlyList<IAsset> Assets
        {
            get { return _assets ?? (_assets = Adaptee.Assets.GetPlainAssets()); }
        }

        public IGradeEquipment Equipment
        {
            get { return new GradeEquipment(Adaptee); }
        }


        public IReadOnlyList<IGradePack> Packs
        {
            get { return Adaptee.Packs.Cast<Legacy.PackCompareItem>().Select(x => new GradePack(x)).ToList(); }
        }
    }

}