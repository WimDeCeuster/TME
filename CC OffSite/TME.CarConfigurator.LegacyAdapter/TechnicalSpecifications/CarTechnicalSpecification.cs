using System;
using TME.CarConfigurator.Interfaces.Enums;
using TME.CarConfigurator.Interfaces.TechnicalSpecifications;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter.TechnicalSpecifications
{
    public class CarTechnicalSpecification : BaseObject, ICarTechnicalSpecification
    {
        #region Dependencies (Adaptee)
        private Legacy.TechnicalSpecification Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public CarTechnicalSpecification(Legacy.TechnicalSpecification adaptee)
            : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public string RawValue
        {
            get { return Adaptee.RawValue; }
        }

        public string Unit
        {
            get { return Adaptee.Unit; }
        }

        public string ValueFormat
        {
            get { return string.Empty; }
        }

        public string Value
        {
            get { return Adaptee.Value; }
        }

        public short ValueSortIndex
        {
            get { return Adaptee.ValueSortIndex; }
        }

        public TypeCode TypeCode
        {
            get { return Adaptee.TypeCode; }
        }

        public bool KeyFeature
        {
            get { return Adaptee.KeyFeature; }
        }

        public bool Brochure
        {
            get { return Adaptee.Brochure; }
        }

        public bool EnergyEfficiencySpecification
        {
            get { return Adaptee.EnergyEfficiencySpecification; }
        }

        public bool QuickSpecification
        {
            get { return Adaptee.QuickSpecification; }
        }

        public bool FullSpecification
        {
            get { return Adaptee.FullSpecification; }
        }

        public bool Compareable
        {
            get { return Adaptee.Compareable; }
        }

        public ValueSelectionType SummaryValueType
        {
            get { return (ValueSelectionType) Adaptee.SummaryValueType; }
        }

        public ValueSelectionType PromotedValueType
        {
            get { return (ValueSelectionType) Adaptee.PromotedValueType; }
        }

        public ICategoryInfo Category
        {
            get { return new CategoryInfo(Adaptee.Category); }
        }
    }
}
