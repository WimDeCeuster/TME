using System;
using System.Linq;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.Exceptions;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
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

        public Car MapCar(Administration.Car car, BodyType bodyType, Engine engine, Transmission transmission, WheelDrive wheelDrive, Steering steering, bool isPreview)
        {
            if (car == null) throw new ArgumentNullException("car");
            if (bodyType == null) throw new ArgumentNullException("bodyType");
            if (engine == null) throw new ArgumentNullException("engine");
            if (transmission == null) throw new ArgumentNullException("transmission");
            if (wheelDrive == null) throw new ArgumentNullException("wheelDrive");
            if (steering == null) throw new ArgumentNullException("steering");
            
            if (car.ShortID == null)
                throw new CorruptDataException(String.Format("Please provide a shortID for car {0}", car.Name));

            var cheapestColourCombinationExcludingVat = car.ColourCombinations
                                               .Where(cc=> cc.Approved)
                                               .OrderBy(cc => cc.ExteriorColour.Price + cc.Upholstery.Price)
                                               .First();

            var cheapestColourCombinationIncludingVat = car.ColourCombinations
                                   .Where(cc => cc.Approved)
                                   .OrderBy(cc => cc.ExteriorColour.VatPrice + cc.Upholstery.VatPrice)
                                   .First();

            var mappedCar = new Car
            {
                BasePrice = new Price
                {
                    ExcludingVat = car.Price,
                    IncludingVat = car.VatPrice
                },
                BodyType = bodyType,
                Engine = engine,
                Transmission = transmission,
                WheelDrive = wheelDrive,
                Steering = steering,
                ConfigVisible = isPreview || car.ConfigVisible,
                FinanceVisible = isPreview || car.FinanceVisible,
                WebVisible = isPreview || car.WebVisible,
                Promoted = car.Promoted,
                ShortID = car.ShortID.Value,
                SortIndex = car.Index,
                StartingPrice = new Price
                {
                    ExcludingVat = car.Price + cheapestColourCombinationExcludingVat.ExteriorColour.Price + cheapestColourCombinationExcludingVat.Upholstery.Price,
                    IncludingVat = car.VatPrice + cheapestColourCombinationIncludingVat.ExteriorColour.VatPrice + cheapestColourCombinationIncludingVat.Upholstery.VatPrice
                },

            };


            return _baseMapper.MapDefaults(mappedCar, car);
        }

        public CarInfo MapCarInfo(Administration.Car car)
        {
            if (car.ShortID == null)
                throw new CorruptDataException(String.Format("Please provide a shortID for car {0}", car.Name));

            return new CarInfo
            {
                Name = car.Translation.Name.DefaultIfEmpty(car.Name),
                ShortID = car.ShortID.Value
            };
        }
        public BodyType CopyBodyType(BodyType bodyType)
        {
            return new BodyType
            {
                ID = bodyType.ID,
                InternalCode = bodyType.InternalCode,
                LocalCode = bodyType.LocalCode,
                Name = bodyType.Name,
                Description = bodyType.Description,
                ToolTip = bodyType.ToolTip,
                FootNote = bodyType.FootNote,
                Labels = bodyType.Labels,
                NumberOfDoors = bodyType.NumberOfDoors,
                NumberOfSeats = bodyType.NumberOfSeats,
                SortIndex = bodyType.SortIndex,
                VisibleIn =
                    bodyType.VisibleIn.Select(
                        x => new VisibleInModeAndView {Mode = x.Mode, View = x.View, CanHaveAssets = true}).ToList()
            };
        }
        public Engine CopyEngine(Engine engine)
        {
            return new Engine
            {
                ID = engine.ID,
                InternalCode = engine.InternalCode,
                LocalCode = engine.LocalCode,
                Name = engine.Name,
                Description = engine.Description,
                ToolTip = engine.ToolTip,
                FootNote = engine.FootNote,
                Labels = engine.Labels,
                SortIndex = engine.SortIndex,
                Brochure = engine.Brochure,
                Category = engine.Category,
                KeyFeature = engine.KeyFeature,
                Type =  engine.Type,
                VisibleIn =
                    engine.VisibleIn.Select(
                        x => new VisibleInModeAndView { Mode = x.Mode, View = x.View, CanHaveAssets = true }).ToList()
            };
        }
        public Transmission CopyTransmission(Transmission transmission)
        {
            return new Transmission
            {
                ID = transmission.ID,
                InternalCode = transmission.InternalCode,
                LocalCode = transmission.LocalCode,
                Name = transmission.Name,
                Description = transmission.Description,
                ToolTip = transmission.ToolTip,
                FootNote = transmission.FootNote,
                Labels = transmission.Labels,
                SortIndex = transmission.SortIndex,
                Brochure = transmission.Brochure,
                NumberOfGears = transmission.NumberOfGears,
                KeyFeature = transmission.KeyFeature,
                Type = transmission.Type,                
                VisibleIn =
                    transmission.VisibleIn.Select(
                        x => new VisibleInModeAndView { Mode = x.Mode, View = x.View, CanHaveAssets = true }).ToList()
            };
        }
        public WheelDrive CopyWheelDrive(WheelDrive wheelDrive)
        {
            return new WheelDrive
            {
                ID = wheelDrive.ID,
                InternalCode = wheelDrive.InternalCode,
                LocalCode = wheelDrive.LocalCode,
                Name = wheelDrive.Name,
                Description = wheelDrive.Description,
                ToolTip = wheelDrive.ToolTip,
                FootNote = wheelDrive.FootNote,
                Labels = wheelDrive.Labels,
                SortIndex = wheelDrive.SortIndex,
                Brochure = wheelDrive.Brochure,
                KeyFeature = wheelDrive.KeyFeature,
                VisibleIn =
                    wheelDrive.VisibleIn.Select(
                        x => new VisibleInModeAndView { Mode = x.Mode, View = x.View, CanHaveAssets = true }).ToList()
            };
        }

        public Grade CopyGrade(Grade grade)
        {
            return new Grade
            {
                ID = grade.ID,
                InternalCode = grade.InternalCode,
                LocalCode = grade.LocalCode,
                Name = grade.Name,
                Description = grade.Description,
                ToolTip = grade.ToolTip,
                FootNote = grade.FootNote,
                Labels = grade.Labels,
                SortIndex = grade.SortIndex,
                BasedUpon = grade.BasedUpon,
                Special = grade.Special,
                StartingPrice = grade.StartingPrice,
                VisibleIn =
                    grade.VisibleIn.Select(
                        x => new VisibleInModeAndView { Mode = x.Mode, View = x.View, CanHaveAssets = true }).ToList()
            };
        }
    }
}
