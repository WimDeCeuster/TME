using System;
using System.Linq;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.Mappers.Exceptions;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class CarMapper : ICarMapper
    {
        readonly IBaseMapper _baseMapper;

        public CarMapper(IBaseMapper baseMapper)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");

            _baseMapper = baseMapper;
        }

        public Car MapCar(
            Administration.Car car,
            BodyType bodyType,
            Engine engine,
            Transmission transmission,
            WheelDrive wheelDrive,
            Steering steering)
        {
            if (car == null) throw new ArgumentNullException("car");
            if (bodyType == null) throw new ArgumentNullException("bodyType");
            if (engine == null) throw new ArgumentNullException("engine");
            if (transmission == null) throw new ArgumentNullException("transmission");
            if (wheelDrive == null) throw new ArgumentNullException("wheelDrive");
            if (steering == null) throw new ArgumentNullException("steering");

            if (car.ShortID == null)
                throw new CorruptDataException(String.Format("Please provide a shortID for car {0}", car.ID));

            var cheapestColourCombination = car.ColourCombinations.OrderBy(cc => cc.ExteriorColour.Price + cc.Upholstery.Price)
                                                                  .First();

            var mappedCar = new Car
            {
                BasePrice = new Price
                {
                    ExcludingVat = car.Price,
                    IncludingVat = car.VatPrice
                },
                BodyType = bodyType,
                ConfigVisible = car.ConfigVisible,
                Engine = engine,
                FinanceVisible = car.FinanceVisible,
                Promoted = car.Promoted,
                ShortID = car.ShortID.Value,
                SortIndex = car.Index,
                StartingPrice = new Price
                {
                    ExcludingVat = car.Price + cheapestColourCombination.ExteriorColour.Price + cheapestColourCombination.Upholstery.Price,
                    IncludingVat = car.VatPrice + cheapestColourCombination.ExteriorColour.VatPrice + cheapestColourCombination.Upholstery.VatPrice
                },
                Steering = steering,
                Transmission = transmission,
                WebVisible = car.WebVisible,
                WheelDrive = wheelDrive
            };


            return _baseMapper.MapDefaults(mappedCar, car, car, car.Name);
        }
    }
}
