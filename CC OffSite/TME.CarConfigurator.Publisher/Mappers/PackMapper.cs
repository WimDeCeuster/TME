using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Administration.Enums;
using TME.CarConfigurator.Publisher.Exceptions;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Packs;
using Car = TME.CarConfigurator.Administration.Car;
using CarPack = TME.CarConfigurator.Repository.Objects.Packs.CarPack;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class PackMapper : IPackMapper
    {
        private readonly IBaseMapper _baseMapper;
        private readonly ICarMapper _carMapper;

        public PackMapper(IBaseMapper baseMapper, ICarMapper carMapper)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");
            if (carMapper == null) throw new ArgumentNullException("carMapper");

            _baseMapper = baseMapper;
            _carMapper = carMapper;
        }

        public GradePack MapGradePack(ModelGenerationGradePack gradePack, ModelGenerationPack generationPack, IReadOnlyCollection<Car> gradeCars)
        {

            if (!gradePack.ShortID.HasValue)
                throw new CorruptDataException(String.Format("Please supply a ShortID for grade pack {0}", gradePack.ID));

            var mappedGradePack = new GradePack
            {
                StandardOn = FindCarsOnWhichPackHasCorrectAvailability(gradeCars, gradePack.ID, Availability.Standard),
                OptionalOn = FindCarsOnWhichPackHasCorrectAvailability(gradeCars, gradePack.ID, Availability.Optional),
                NotAvailableOn = FindCarsOnWhichPackHasCorrectAvailability(gradeCars, gradePack.ID, Availability.NotAvailable),

                ShortID = gradePack.ShortID.Value,
                GradeFeature = gradePack.GradeFeature,
                OptionalGradeFeature = gradePack.OptionalGradeFeature,
            };


            mappedGradePack.NotAvailable = mappedGradePack.CalculateNotAvailable();
            mappedGradePack.Optional = mappedGradePack.CalculateOptional();
            mappedGradePack.Standard = mappedGradePack.CalculateStandard();
            
            _baseMapper.MapDefaultsWithSort(mappedGradePack, generationPack);

            mappedGradePack.LocalCode = gradePack.LocalCode; // basemapper falls back to internalcode by default, but this shouldn't happen for grade packs

            return mappedGradePack;
        }

        private IReadOnlyList<CarInfo> FindCarsOnWhichPackHasCorrectAvailability(IEnumerable<Car> gradeCars, Guid packID, Availability availability)
        {
            var matchingCars = gradeCars.Where(c =>
            {
                var carPack = c.Packs[packID];
                return carPack != null && carPack.Availability == availability;
            });

            return matchingCars.Select(c => _carMapper.MapCarInfo(c)).ToList();
        }


        public CarPack MapCarPack(Administration.CarPack carPack)
        {
            if (carPack.ShortID == null)
                throw new CorruptDataException(String.Format("Please provide a short id for car pack {0}", carPack.ID));

            var gradePack = carPack.Car.Generation.Grades[carPack.Car.GradeID].Packs[carPack.ID];

            var mappedCarPack = new CarPack
            {
                ID = carPack.ID,
                GradeFeature = gradePack.GradeFeature,
                InternalCode = carPack.Code,
                LocalCode = carPack.LocalCode,
                Price = new Repository.Objects.Core.Price
                {
                    ExcludingVat = carPack.Price,
                    IncludingVat = carPack.VatPrice
                },
                ShortID = carPack.ShortID.Value,
                SortIndex = carPack.Index,
                Optional = carPack.Availability == Availability.Optional,
                Standard = carPack.Availability == Availability.Standard,
                OptionalGradeFeature = gradePack.GradeFeature && carPack.Availability == Availability.Optional
            };

            return _baseMapper.MapTranslation(mappedCarPack, carPack.Translation, carPack.Name);
        }
    }
}