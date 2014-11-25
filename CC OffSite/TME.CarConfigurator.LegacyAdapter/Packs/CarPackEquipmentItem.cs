using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Enums;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Interfaces.Rules;
using TME.CarConfigurator.LegacyAdapter.Colours;
using TME.CarConfigurator.LegacyAdapter.Equipment;
using TME.CarConfigurator.LegacyAdapter.Extensions;
using ExteriorColour = TME.CarConfigurator.LegacyAdapter.Equipment.ExteriorColour;
using IExteriorColour = TME.CarConfigurator.Interfaces.Equipment.IExteriorColour;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter.Packs
{

    public abstract class CarPackEquipmentItem : BaseObject, ICarPackEquipmentItem
    {

        #region Dependencies (Adaptee)
        private Legacy.CarPackEquipmentItem Adaptee
        {
            get;
            set;
        }
        private Legacy.CarEquipmentItem StandAloneItemOfTheAdaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        protected CarPackEquipmentItem(Legacy.CarPackEquipmentItem adaptee, Legacy.CarEquipmentItem standAloneItemOfTheAdaptee)
            : base(adaptee)
        {
            Adaptee = adaptee;
            StandAloneItemOfTheAdaptee = standAloneItemOfTheAdaptee;
        }
        #endregion

        public int ShortID
        {
            get { return Adaptee.ShortID; }
        }

        public string InternalName
        {
            get { return Adaptee.InternalName; }
        }

        public string PartNumber
        {
            get { return Adaptee.PartNumber; }
        }

        public string Path
        {
            get { return Adaptee.Path; }
        }

        public bool KeyFeature
        {
            get
            {
                return StandAloneItemOfTheAdaptee != null && StandAloneItemOfTheAdaptee.KeyFeature;
            }
        }

        public bool GradeFeature
        {
            get
            {
                return StandAloneItemOfTheAdaptee != null && StandAloneItemOfTheAdaptee.GradeFeature;
            }
        }

        public bool OptionalGradeFeature
        {
            get
            {
                return StandAloneItemOfTheAdaptee != null && StandAloneItemOfTheAdaptee.OptionalGradeFeature;
            }
        }

        public bool Brochure
        {
            get { return Adaptee.Brochure; }
        }

        public Visibility Visibility
        {
            get { return Adaptee.Visibility.ToVisibility(); }
        }

        public IBestVisibleIn BestVisibleIn
        {
            get
            {
                return StandAloneItemOfTheAdaptee==null ? new BestVisibleIn() : new BestVisibleIn(StandAloneItemOfTheAdaptee.BestVisibleIn);
            }
        }

        public ICategoryInfo Category
        {
            get { return new CategoryInfo(Adaptee.Category); }
        }


        public IExteriorColour ExteriorColour
        {
            get
            {
                var colour = Adaptee.Colour;
                return colour.IsEmpty() ? null : new ExteriorColour(colour);
            }
        }

        public IReadOnlyList<ILink> Links
        {
            get
            {
                if (StandAloneItemOfTheAdaptee==null) return  new List<ILink>();
                return StandAloneItemOfTheAdaptee.Links.Cast<TMME.CarConfigurator.Link>().Select(x => new Link(x)).ToList();
            }
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

        public IReadOnlyList<IExteriorColourInfo> AvailableForExteriorColours
        {
            get
            {
                if (StandAloneItemOfTheAdaptee == null) return null;
                if (StandAloneItemOfTheAdaptee.AvailableForExteriorColours == null) return null;
                return
                    StandAloneItemOfTheAdaptee.AvailableForExteriorColours.Cast<TMME.CarConfigurator.CarExteriorColour>()
                        .Select(x => new ExteriorColourInfo(x))
                        .ToList();
            }
        }

        public IReadOnlyList<IUpholsteryInfo> AvailableForUpholsteries
        {
            get
            {
                if (StandAloneItemOfTheAdaptee == null) return null;
                if (StandAloneItemOfTheAdaptee.AvailableForUpholsteries == null) return null;
                return
                    StandAloneItemOfTheAdaptee.AvailableForUpholsteries.Cast<TMME.CarConfigurator.CarUpholstery>()
                        .Select(x => new UpholsteryInfo(x))
                        .ToList();
            }
        }


        public ColouringModes ColouringModes
        {
            get { return Adaptee.ColouringModes.ToColouringModes(); }
        }


        IReadOnlyList<ILink> IEquipmentItem.Links
        {
            get
            {
                if (StandAloneItemOfTheAdaptee == null) return new List<ILink>();
                return StandAloneItemOfTheAdaptee.Links.Cast<TMME.CarConfigurator.Link>().Select(x => new Link(x)).ToList();
            }
        }


        public IRuleSets Rules
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}