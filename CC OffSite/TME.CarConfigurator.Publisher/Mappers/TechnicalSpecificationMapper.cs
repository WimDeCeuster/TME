using System;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.TechnicalSpecifications;
using ValueSelectionType = TME.CarConfigurator.Repository.Objects.Enums.ValueSelectionType;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class TechnicalSpecificationMapper : ITechnicalSpecificationMapper
    {
        private readonly IBaseMapper _baseMapper;
        private readonly ICategoryMapper _categoryMapper;

        public TechnicalSpecificationMapper(IBaseMapper baseMapper, ICategoryMapper categoryMapper)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");
            if (categoryMapper == null) throw new ArgumentNullException("categoryMapper");
            _baseMapper = baseMapper;
            _categoryMapper = categoryMapper;

        }
        
        public CarTechnicalSpecification MapTechnicalSpecification(CarSpecification carSpecification, Specification specification, Unit unit)
        {
            var value = GetValue(carSpecification);

            var mappedSpecification = new CarTechnicalSpecification
            {
                RawValue = value,
                Unit = unit.AlternateName,
                ValueFormat = specification.SystemValueFormat,

                Brochure = carSpecification.Brochure,
                KeyFeature = carSpecification.KeyFeature,
                Category = _categoryMapper.MapSpecificationCategoryInfo(specification.Category),
                EnergyEfficiencySpecification = carSpecification.EnergyEfficiencySpecification,
                FullSpecification = carSpecification.FullSpecification,
                QuickSpecification = carSpecification.QuickSpecification,
                TypeCode = carSpecification.TypeCode,

                Compareable = specification.Compareable,
                PromotedValueType = (ValueSelectionType) specification.PromotedValue,
                SummaryValueType = (ValueSelectionType) specification.SummaryValue,
                ValueSortIndex = specification.GetSortOrderForValue(value),
                SortIndex = specification.Index
            };

            return _baseMapper.MapDefaults(
                mappedSpecification, 
                carSpecification.GenerationSpecification, 
                specification.Translation.Labels
                );
            
        }
        private static string GetValue(CarSpecification carSpecification)
        {
            var context = MyContext.GetContext();
            var carValues = carSpecification.Values.GetSpecificOrDefaultValue(context.CountryCode, context.LanguageCode);

            if (carValues.Value != null) return carValues.Value;
            if (carSpecification.IsMasterSpecification() && carValues.MasterValue != null) return carValues.MasterValue;
            return string.Empty;
        }


    }
}