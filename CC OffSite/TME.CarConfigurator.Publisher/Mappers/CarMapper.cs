﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;
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

        public Car MapCar(Administration.Car car, Repository.Objects.BodyType bodyType, Repository.Objects.Engine engine, Repository.Objects.Transmission transmission)
        {
            var cheapestColourCombination = car.ColourCombinations.OrderBy(cc => cc.ExteriorColour.Price + cc.Upholstery.Price)
                                                                  .First();

            // TODO: shortid (needs to be implemented by admin library)
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
                ShortID = 0,
                SortIndex = car.Index,
                StartingPrice = new Price
                {
                    ExcludingVat = car.Price + cheapestColourCombination.ExteriorColour.Price + cheapestColourCombination.Upholstery.Price,
                    IncludingVat = car.VatPrice + cheapestColourCombination.ExteriorColour.VatPrice + cheapestColourCombination.Upholstery.VatPrice
                },
                ToolTip = car.Translation.ToolTip,
                Transmission = transmission,
                WebVisible = car.WebVisible
            };
        }
    }
}
