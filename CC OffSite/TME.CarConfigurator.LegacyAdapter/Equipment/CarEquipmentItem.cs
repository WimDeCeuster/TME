using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Enums;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.LegacyAdapter.Extensions;

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
        private TMME.CarConfigurator.Car CarOfAdaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        protected CarEquipmentItem(TMME.CarConfigurator.CarEquipmentItem adaptee, TMME.CarConfigurator.Car carOfAdaptee)
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
                if (colour.IsEmpty()) return null;

                var carColour = CarOfAdaptee.Colours.ExteriorColours[colour.ID];
                return carColour == null
                    ? (IExteriorColour)new Colours.ExteriorColour(colour)
                    : new Colours.CarExteriorColour(carColour);
            }
        }
        public IEnumerable<ILink> Links
        {
            get { return Adaptee.Links.Cast<TMME.CarConfigurator.Link>().Select(x => new Link(x)); }
        }

        public bool Standard
        {
            get { return Adaptee.Standard; }
        }

        public bool Optional
        {
            get { return Adaptee.Optional; }
        }

        public abstract IPrice TotalPrice { get; }
    }
}
