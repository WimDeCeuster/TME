using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Core;
using Car = TME.CarConfigurator.Repository.Objects.Car;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class GradeMapper : IGradeMapper
    {
        private readonly IBaseMapper _baseMapper;
        private readonly IAssetSetMapper _assetSetMapper;
        private readonly ILabelMapper _labelMapper;

        public GradeMapper(IBaseMapper baseMapper, IAssetSetMapper assetSetMapper,ILabelMapper labelMapper)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");
            if (assetSetMapper == null) throw new ArgumentNullException("assetSetMapper");
            if (labelMapper == null) throw new ArgumentNullException("labelMapper");

            _baseMapper = baseMapper;
            _assetSetMapper = assetSetMapper;
            _labelMapper = labelMapper;
        }

        public Grade MapGenerationGrade(ModelGenerationGrade generationGrade, IEnumerable<Car> cars)
        {
            var gradeCars = generationGrade.Cars().ToArray();
            var cheapestCar = cars.Where(car => gradeCars.Any(gradeCar => gradeCar.ID == car.ID))
                .OrderBy(car => car.StartingPrice.IncludingVat)
                .First();

            var mappedGrade = new Grade
            {
                BasedUponGradeID = generationGrade.BasedUpon.ID,
                Special = generationGrade.Special,
                StartingPrice = new Price
                {
                    ExcludingVat = cheapestCar.BasePrice.ExcludingVat,
                    IncludingVat = cheapestCar.BasePrice.IncludingVat
                },
                VisibleIn = _assetSetMapper.GetVisibility(generationGrade.AssetSet).ToList()
            };

            return _baseMapper.MapDefaultsWithSort(mappedGrade, generationGrade);
        }

        public Grade MapSubModelGrade(ModelGenerationGrade grade, ModelGenerationSubModel subModel,IEnumerable<Car> cars)
        {
            var generationGradeSubModel = grade.SubModels.Where(s => s.Name == subModel.Name).Single(s => s.SubModel.ID == subModel.ID);
            var gradeCars = grade.Cars().Where(car => subModel.Cars().Any(subModelCar => subModelCar.ID == car.ID)).ToArray();
            var cheapestCar = cars.Where(car => gradeCars.Any(gradeCar => gradeCar.ID == car.ID))
                .OrderBy(car => car.StartingPrice.IncludingVat)
                .First();

            var mappedGrade = new Grade
            {
                BasedUponGradeID = grade.BasedUpon.ID,
                Special = grade.Special,
                StartingPrice = new Price()
                {
                    ExcludingVat = cheapestCar.BasePrice.ExcludingVat,
                    IncludingVat = cheapestCar.BasePrice.IncludingVat
                },
                VisibleIn = _assetSetMapper.GetVisibility(grade.AssetSet).ToList()
            };

            var mappedGradeForSubModelWithDefaults = _baseMapper.MapDefaultsWithSort(mappedGrade, grade);

            mappedGradeForSubModelWithDefaults.Name = SetTheCorrectSubModelGradeName(generationGradeSubModel, grade);
            mappedGradeForSubModelWithDefaults.LocalCode = String.Empty;
            mappedGradeForSubModelWithDefaults.InternalCode = grade.Code + "-" + subModel.Code;

            SetTheCorrectSubModelGradeLabels(grade, mappedGradeForSubModelWithDefaults, generationGradeSubModel);


            return mappedGradeForSubModelWithDefaults;
        }

        private void SetTheCorrectSubModelGradeLabels(ModelGenerationGrade grade, 
            Grade mappedGradeForSubModelWithDefaults,
            ModelGenerationGradeSubModel generationGradeSubModel)
        {
            mappedGradeForSubModelWithDefaults.Labels =
                generationGradeSubModel.Translation.Labels.Select(_labelMapper.MapLabel)
                    .Where(adminLabel => !String.IsNullOrWhiteSpace(adminLabel.Value))
                    .ToList();

            if (mappedGradeForSubModelWithDefaults.Labels.Count == 0)
            {
                mappedGradeForSubModelWithDefaults.Labels = grade.Translation.Labels.Select(_labelMapper.MapLabel)
                    .Where(adminLabel => !String.IsNullOrWhiteSpace(adminLabel.Value)).ToList();
            }
        }

        private static String SetTheCorrectSubModelGradeName(ModelGenerationGradeSubModel subModel, ModelGenerationGrade grade)
        {
            if (!String.IsNullOrEmpty(subModel.Translation.Name))
                return subModel.Translation.Name;
            return !String.IsNullOrEmpty(grade.Translation.Name) ? grade.Translation.Name : grade.Name;
        }
    }
}
