using System;
using System.Globalization;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.Enums;
using TME.CarConfigurator.Interfaces.TechnicalSpecifications;
using RepositoryCarTechnicalSpecification = TME.CarConfigurator.Repository.Objects.TechnicalSpecifications.CarTechnicalSpecification;

namespace TME.CarConfigurator.TechnicalSpecifications
{
    public class CarTechnicalSpecification : BaseObject<RepositoryCarTechnicalSpecification>, ICarTechnicalSpecification
    {

        private readonly string _rawValue;
        private readonly string _value;
        private readonly ICategoryInfo _categoryInfo;

        public CarTechnicalSpecification(RepositoryCarTechnicalSpecification repositoryObject) : base(repositoryObject)
        {
            _rawValue = GetRawValueForCurrentCulture(repositoryObject);
            _value = String.Format(repositoryObject.ValueFormat, _rawValue, repositoryObject.Unit);
            _categoryInfo = new CategoryInfo(repositoryObject.Category);
        }

        private static string GetRawValueForCurrentCulture(RepositoryCarTechnicalSpecification repositoryObject)
        {
            if (repositoryObject.TypeCode != TypeCode.Decimal) return repositoryObject.RawValue;
            return repositoryObject.RawValue
                .Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                .Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        }

        public string RawValue
        {
            get { return _rawValue; }
        }

        public string Unit
        {
            get { return RepositoryObject.Unit; }
        }

        public string ValueFormat
        {
            get { return RepositoryObject.ValueFormat; }
        }

        public string Value
        {
            get { return _value; }
        }

        public short ValueSortIndex
        {
            get { return RepositoryObject.ValueSortIndex; }
        }

        public TypeCode TypeCode
        {
            get { return RepositoryObject.TypeCode; }
        }

        public bool KeyFeature
        {
            get { return RepositoryObject.KeyFeature; }
        }

        public bool Brochure
        {
            get { return RepositoryObject.Brochure; }
        }

        public bool EnergyEfficiencySpecification
        {
            get { return RepositoryObject.EnergyEfficiencySpecification; }
        }

        public bool QuickSpecification
        {
            get { return RepositoryObject.QuickSpecification; }
        }

        public bool FullSpecification
        {
            get { return RepositoryObject.FullSpecification; }
        }

        public bool Compareable
        {
            get { return RepositoryObject.Compareable; }
        }

        public ValueSelectionType SummaryValueType
        {
            get { return (ValueSelectionType) RepositoryObject.SummaryValueType; }
        }

        public ValueSelectionType PromotedValueType
        {
            get { return (ValueSelectionType)RepositoryObject.PromotedValueType; }
        }

        public ICategoryInfo Category
        {
            get { return _categoryInfo; }
        }
    }
}
