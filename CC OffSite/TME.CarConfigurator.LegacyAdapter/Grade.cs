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
        #endregion

        #region Constructor
        public Grade(Legacy.Grade adaptee)
            : base(adaptee)
        {
            Adaptee = adaptee;
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

        public IReadOnlyList<IVisibleInModeAndView> VisibleIn
        {
            get { return Adaptee.Assets.GetVisibleInModeAndViews(); }
        }

        public IReadOnlyList<IAsset> Assets
        {
            get { return Adaptee.Assets.GetPlainAssets(); }
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