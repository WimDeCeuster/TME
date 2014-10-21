using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class SubModelMapper : ISubModelMapper
    {
        private readonly IBaseMapper _baseMapper;

        public SubModelMapper(IBaseMapper baseMapper)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");

            _baseMapper = baseMapper;
        }

        public SubModel MapSubModel(ModelGenerationSubModel modelGenerationSubModel, IEnumerable<Administration.Car> cars)
        {
            var subModelCars =
                modelGenerationSubModel.Generation.Cars.ToArray();

            var cheapestCar = cars.Where(car => subModelCars.Any(subModelCar => subModelCar.ID == car.ID))
                .OrderBy(car => car.Price)
                .First();

            var mappedSubModel = new SubModel()
            {
                StartingPrice = new Price()
                {
                    ExcludingVat = cheapestCar.Price,
                    IncludingVat = cheapestCar.VatPrice,
                }
            };

            return _baseMapper.MapDefaultsWithSort(mappedSubModel, modelGenerationSubModel, modelGenerationSubModel, modelGenerationSubModel.Name);
        }
    }
}