using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.Mappers.Exceptions;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class CarMapper : ICarMapper
    {
        ILabelMapper _labelMapper;

        public CarMapper(ILabelMapper labelMapper)
        {
            if (labelMapper == null) throw new ArgumentNullException("labelMapper");

            _labelMapper = labelMapper;
        }

        public Car MapCar(Administration.Car car, Repository.Objects.BodyType bodyType, Repository.Objects.Engine engine, Repository.Objects.Transmission transmission, Repository.Objects.WheelDrive wheelDrive, Repository.Objects.Steering steering)
        {
            if (car.ShortID == null)
                throw new CorruptDataException(String.Format("Please provide a shortID for car {0}", car.ID));

            var cheapestColourCombination = car.ColourCombinations.OrderBy(cc => cc.ExteriorColour.Price + cc.Upholstery.Price)
                                                                  .First();

            return new Car
            {
                BasePrice = new Price
                {
                    ExcludingVat = car.Price,
                    IncludingVat = car.VatPrice
                },
                BodyType = bodyType,
                ConfigVisible = car.ConfigVisible,
                Description = car.Translation.Description,
                Engine = engine,
                FinanceVisible = car.FinanceVisible,
                FootNote = car.Translation.FootNote,
                ID = car.ID,
                InternalCode = car.BaseCode,
                Labels = car.Translation.Labels.Select(_labelMapper.MapLabel).ToList(),
                LocalCode = car.LocalCode.DefaultIfEmpty(car.BaseCode),
                Name = car.Translation.Name.DefaultIfEmpty(car.Name),
                Promoted = car.Promoted,
                ShortID = car.ShortID.Value,
                SortIndex = car.Index,
                StartingPrice = new Price
                {
                    ExcludingVat = car.Price + cheapestColourCombination.ExteriorColour.Price + cheapestColourCombination.Upholstery.Price,
                    IncludingVat = car.VatPrice + cheapestColourCombination.ExteriorColour.VatPrice + cheapestColourCombination.Upholstery.VatPrice
                },
                Steering = steering,
                ToolTip = car.Translation.ToolTip,
                Transmission = transmission,
                WebVisible = car.WebVisible,
                WheelDrive = wheelDrive
            };
        }
    }
}
