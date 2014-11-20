using System;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Enums;

namespace TME.CarConfigurator.Interfaces.TechnicalSpecifications
{
    public interface ICarTechnicalSpecification : IBaseObject
    {
        string RawValue { get; }
        string Unit { get; }
        string ValueFormat { get; }

        string Value { get; }
        short ValueSortIndex { get; }
        
        TypeCode TypeCode { get; }

        bool KeyFeature { get; }
        bool Brochure { get; }
        bool EnergyEfficiencySpecification { get; }
        bool QuickSpecification { get; }
        bool FullSpecification { get; }
        bool Compareable { get; }

        ValueSelectionType SummaryValueType { get; }
        ValueSelectionType PromotedValueType { get; }
        ICategoryInfo Category { get; }
    }
}
