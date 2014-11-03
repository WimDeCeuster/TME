using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.LegacyAdapter.Extensions;
using Visibility = TME.CarConfigurator.Interfaces.Enums.Visibility;

namespace TME.CarConfigurator.LegacyAdapter.Equipment
{
    public abstract class GradeEquipmentItem : BaseObject, IGradeEquipmentItem
    {

        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.EquipmentCompareItem Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        protected GradeEquipmentItem(TMME.CarConfigurator.EquipmentCompareItem adaptee)
            : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        protected TMME.CarConfigurator.Car GetCar()
        {
            if (Adaptee.StandardOn.Count > 0) return Adaptee.StandardOn[0];
            if (Adaptee.OptionalOn.Count > 0) return Adaptee.OptionalOn[0];
            return Adaptee.NotAvailableOn[0];
        }
        protected TMME.CarConfigurator.CarEquipmentItem GetCarEquipmentItem()
        {
            return GetCar().Equipment[Adaptee.ID];
        }
        public int ShortID
        {
            get { return GetCarEquipmentItem().ShortID; }
        }

        public string InternalName
        {
            get { return Adaptee.InternalName; }
        }

        public string PartNumber
        {
            get {  return Adaptee.PartNumber; }
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
            get { return GetCarEquipmentItem().Brochure;}
        }

        public Visibility Visibility
        {
            get { return GetCarEquipmentItem().Visibility.ToVisibility(); }
        }

        public IBestVisibleIn BestVisibleIn
        {
            get { return new BestVisibleIn(GetCarEquipmentItem().BestVisibleIn); }
        }

        public ICategoryInfo Category
        {
            get { return new CategoryInfo(Adaptee.Category); }
        }

        public IExteriorColour ExteriorColour
        {
            get
            {
                var colour = GetCarEquipmentItem().Colour;
                if (colour.IsEmpty()) return null;

                var carColour = GetCar().Colours.ExteriorColours[colour.ID];
                return carColour == null
                    ? (IExteriorColour) new Colours.ExteriorColour(colour)
                    : new Colours.CarExteriorColour(carColour);
            }
        }
        public IEnumerable<ILink> Links
        {
            get { return GetCarEquipmentItem().Links.Cast<TMME.CarConfigurator.Link>().Select(x => new Link(x)); }
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

        public IReadOnlyList<ICarInfo> StandardOn
        {
            get { return Adaptee.StandardOn.Cast<TMME.CarConfigurator.Car>().Select(x => new CarInfo(x)).ToList(); }
        }

        public IReadOnlyList<ICarInfo> OptionalOn
        {
            get { return Adaptee.OptionalOn.Cast<TMME.CarConfigurator.Car>().Select(x => new CarInfo(x)).ToList(); }
        }

        public IReadOnlyList<ICarInfo> NotAvailableOn
        {
            get
            {
                return Adaptee.NotAvailableOn.Cast<TMME.CarConfigurator.Car>().Select(x => new CarInfo(x)).ToList(); 
            }
        }
    }
}