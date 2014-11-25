using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Rules;
using TME.CarConfigurator.LegacyAdapter.Colours;
using TME.CarConfigurator.LegacyAdapter.Extensions;
using IExteriorColour = TME.CarConfigurator.Interfaces.Equipment.IExteriorColour;
using IPrice = TME.CarConfigurator.Interfaces.Core.IPrice;
using Visibility = TME.CarConfigurator.Interfaces.Enums.Visibility;

namespace TME.CarConfigurator.LegacyAdapter.Equipment
{
    public abstract class CarEquipmentItem : BaseObject, ICarEquipmentItem
    {

        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.CarEquipmentItem Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        protected CarEquipmentItem(TMME.CarConfigurator.CarEquipmentItem adaptee)
            : base(adaptee)
        {
            Adaptee = adaptee;
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
            get { return Adaptee.KeyFeature; }
        }

        public bool GradeFeature
        {
            get { return Adaptee.GradeFeature; }
        }

        public bool OptionalGradeFeature
        {
            get { return Adaptee.OptionalGradeFeature; }
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
            get { return new BestVisibleIn(Adaptee.BestVisibleIn); }
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
            get { return Adaptee.Links.Cast<TMME.CarConfigurator.Link>().Select(x => new Link(x)).ToList(); }
        }

        public bool Standard
        {
            get { return Adaptee.Standard; }
        }

        public bool Optional
        {
            get { return Adaptee.Optional; }
        }

        public abstract IPrice Price { get; }

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
                if (Adaptee.AvailableForExteriorColours == null) return new List<IExteriorColourInfo>();
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
                if (Adaptee.AvailableForUpholsteries == null) return new List<IUpholsteryInfo>();
                return
                    Adaptee.AvailableForUpholsteries.Cast<TMME.CarConfigurator.CarUpholstery>()
                        .Select(x => new UpholsteryInfo(x))
                        .ToList();
            }
        }

        
        public IRuleSets Rules
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}
