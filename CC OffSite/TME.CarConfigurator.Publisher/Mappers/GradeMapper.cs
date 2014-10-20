using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class GradeMapper : IGradeMapper
    {
        IBaseMapper _baseMapper;

        public GradeMapper(IBaseMapper baseMapper)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");

            _baseMapper = baseMapper;
        }

        public Grade MapGrade(Administration.ModelGenerationGrade generationGrade, IEnumerable<Car> cars)
        {
            var gradeCars = generationGrade.Cars().ToArray();
            var cheapestCar = cars.Where(car => gradeCars.Any(gradeCar => gradeCar.ID == car.ID))
                                  .OrderBy(car => car.StartingPrice.ExcludingVat)
                                  .First();

            var mappedGrade = new Grade
            {
                Special = generationGrade.Special,
                StartingPrice = new Price
                {
                    ExcludingVat = cheapestCar.StartingPrice.ExcludingVat,
                    IncludingVat = cheapestCar.StartingPrice.IncludingVat
                }
            };

            return _baseMapper.MapDefaultsWithSort(mappedGrade, generationGrade, generationGrade, generationGrade.Name);
        }
    }
}
