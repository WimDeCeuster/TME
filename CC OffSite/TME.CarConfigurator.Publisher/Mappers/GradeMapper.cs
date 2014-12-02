using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Core;
using Car = TME.CarConfigurator.Repository.Objects.Car;
using GradeInfo = TME.CarConfigurator.Repository.Objects.GradeInfo;

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
            var repositoryCarsOfGrade = cars.Where(car => gradeCars.Any(gradeCar => gradeCar.ID == car.ID)).ToList();
            var cheapestCarIncludingVat = repositoryCarsOfGrade
                .OrderBy(car => car.StartingPrice.IncludingVat)
                .First();
            var cheapestCarExcludingVat = repositoryCarsOfGrade
                .OrderBy(car => car.StartingPrice.ExcludingVat)
                .First();

            var mappedGrade = GetMappedGrade(generationGrade, cheapestCarExcludingVat, cheapestCarIncludingVat, false);

            return _baseMapper.MapDefaultsWithSort(mappedGrade, generationGrade);
        }

        public Grade MapSubModelGrade(ModelGenerationGrade grade, ModelGenerationSubModel subModel, IEnumerable<Car> cars)
        {
            var generationGradeSubModel = grade.SubModels.Where(s => s.Name == subModel.Name).Single(s => s.SubModel.ID == subModel.ID);
            var gradeCars = grade.Cars().Where(car => subModel.Cars().Any(subModelCar => subModelCar.ID == car.ID)).ToArray();
            var repositoryCarsOfGrade = cars.Where(car => gradeCars.Any(gradeCar => gradeCar.ID == car.ID)).ToList();
            var cheapestCarIncludingVat = repositoryCarsOfGrade
                .OrderBy(car => car.StartingPrice.IncludingVat)
                .First();
            var cheapestCarExcludingVat = repositoryCarsOfGrade
                .OrderBy(car => car.StartingPrice.ExcludingVat)
                .First();

            var mappedGrade = GetMappedGrade(grade, cheapestCarExcludingVat, cheapestCarIncludingVat, false);

            var mappedGradeForSubModelWithDefaults = _baseMapper.MapDefaultsWithSort(mappedGrade, grade);

            mappedGradeForSubModelWithDefaults.Name = SetTheCorrectSubModelGradeName(generationGradeSubModel, grade);
            mappedGradeForSubModelWithDefaults.LocalCode = String.Empty;
            mappedGradeForSubModelWithDefaults.InternalCode = grade.Code + "-" + subModel.Code;

            mappedGradeForSubModelWithDefaults.Labels = _labelMapper.MapLabels(generationGradeSubModel.Translation.Labels, grade.Translation.Labels);

            SetTheCorrectSubModelGradeLabels(grade, mappedGradeForSubModelWithDefaults, generationGradeSubModel);

            return mappedGradeForSubModelWithDefaults;
        }

        private Grade GetMappedGrade(ModelGenerationGrade grade, Car cheapestCarExcludingVat, Car cheapestCarIncludingVat,bool canHaveAssets)
        {
            var mappedGrade = new Grade
            {
                BasedUpon = GetBasedUponGradeInfo(grade),
                Special = grade.Special,
                StartingPrice = new Price
                {
                    ExcludingVat = cheapestCarExcludingVat.StartingPrice.ExcludingVat,
                    IncludingVat = cheapestCarIncludingVat.StartingPrice.IncludingVat
                },
                VisibleIn = _assetSetMapper.GetVisibility(grade.AssetSet, canHaveAssets).ToList()
            };
            return mappedGrade;
        }

        private static GradeInfo GetBasedUponGradeInfo(ModelGenerationGrade grade)
        {
            var basedUponGrade = grade.Generation.Grades.FirstOrDefault(x => x.ID == grade.BasedUpon.ID);
            var basedUponGradeInfo = (basedUponGrade == null
                ? null
                : new GradeInfo
                {
                    ID = (basedUponGrade.ID),
                    Name = (basedUponGrade.Translation.Name.DefaultIfEmpty(basedUponGrade.Name))
                });
            return basedUponGradeInfo;
        }

        private void SetTheCorrectSubModelGradeLabels(ModelGenerationGrade grade, 
            Grade mappedGradeForSubModelWithDefaults,
            ModelGenerationGradeSubModel generationGradeSubModel)
        {
            mappedGradeForSubModelWithDefaults.Labels = _labelMapper.MapLabels(generationGradeSubModel.Translation.Labels);

            if (mappedGradeForSubModelWithDefaults.Labels.Count == 0)
                mappedGradeForSubModelWithDefaults.Labels = _labelMapper.MapLabels(grade.Translation.Labels);
            
        }

        private static String SetTheCorrectSubModelGradeName(ModelGenerationGradeSubModel subModel, ModelGenerationGrade grade)
        {
            if (!String.IsNullOrEmpty(subModel.Translation.Name))
                return subModel.Translation.Name;
            return !String.IsNullOrEmpty(grade.Translation.Name) ? grade.Translation.Name : grade.Name;
        }
    }
}
