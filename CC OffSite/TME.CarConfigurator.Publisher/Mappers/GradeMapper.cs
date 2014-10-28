using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Core;
using Car = TME.CarConfigurator.Repository.Objects.Car;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class GradeMapper : IGradeMapper
    {
        private IBaseMapper _baseMapper;
        private readonly IAssetSetMapper _assetSetMapper;

        public GradeMapper(IBaseMapper baseMapper, IAssetSetMapper assetSetMapper)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");
            if (assetSetMapper == null) throw new ArgumentNullException("assetSetMapper");

            _baseMapper = baseMapper;
            _assetSetMapper = assetSetMapper;
        }

        public Grade MapGrade(Administration.ModelGenerationGrade generationGrade, IEnumerable<Car> cars)
        {
            var gradeCars = generationGrade.Cars().ToArray();
            var cheapestCar = cars.Where(car => gradeCars.Any(gradeCar => gradeCar.ID == car.ID))
                .OrderBy(car => car.StartingPrice.ExcludingVat)
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

            return _baseMapper.MapDefaultsWithSort(mappedGrade, generationGrade, generationGrade);
        }
    }
}
