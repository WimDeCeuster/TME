using System;
using TME.CarConfigurator.Repository.Objects.Core;
using TME.CarConfigurator.Repository.Objects.Enums;

namespace TME.CarConfigurator.Repository.Objects.TechnicalSpecifications
{
    public class CarTechnicalSpecification : BaseObject
    {
        public string RawValue { get; set; }
        public string Unit { get; set; }
        public string ValueFormat { get; set; }
        public short ValueSortIndex { get; set; }

        public TypeCode TypeCode { get; set; }

        public bool KeyFeature { get; set; }
        public bool Brochure { get; set; }
        public bool EnergyEfficiencySpecification { get; set; }
        public bool QuickSpecification { get; set; }
        public bool FullSpecification { get; set; }
        public bool Compareable { get; set; }

        public ValueSelectionType SummaryValueType { get; set; }
        public ValueSelectionType PromotedValueType { get; set; }
        public CategoryInfo Category { get; set; }
    }
}
