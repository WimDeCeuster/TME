using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Administration.Enums;
using TME.CarConfigurator.Publisher.Exceptions;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Core;
using TME.CarConfigurator.Repository.Objects.Packs;
using Car = TME.CarConfigurator.Administration.Car;
using CarPack = TME.CarConfigurator.Repository.Objects.Packs.CarPack;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class PackMapper : IPackMapper
    {
        private readonly IBaseMapper _baseMapper;
        private readonly ICarMapper _carMapper;
        private readonly IColourMapper _colourMapper;
        private readonly IEquipmentMapper _equipmentMapper;

        public PackMapper(IBaseMapper baseMapper, ICarMapper carMapper, IColourMapper colourMapper, IEquipmentMapper equipmentMapper)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");
            if (carMapper == null) throw new ArgumentNullException("carMapper");
            if (colourMapper == null) throw new ArgumentNullException("colourMapper");
            if (equipmentMapper == null) throw new ArgumentNullException("equipmentMapper");

            _baseMapper = baseMapper;
            _carMapper = carMapper;
            _colourMapper = colourMapper;
            _equipmentMapper = equipmentMapper;
        }

        public GradePack MapGradePack(ModelGenerationGradePack gradePack, ModelGenerationPack generationPack, IReadOnlyCollection<Car> gradeCars)
        {

            if (!gradePack.ShortID.HasValue)
                throw new CorruptDataException(String.Format("Please supply a ShortID for grade pack {0}", gradePack.Name));

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

        public CarPack MapCarPack(Administration.CarPack carPack, EquipmentGroups groups, EquipmentCategories categories, bool isPreview, string assetUrl)
        {
            if (carPack.ShortID == null)
                throw new CorruptDataException(String.Format("Please provide a short id for car pack {0}", carPack.Name));

            var gradePack = carPack.Car.Generation.Grades[carPack.Car.GradeID].Packs[carPack.ID];

            var accessories = carPack.Equipment.OfType<Administration.CarPackAccessory>().Where(x => x.Availability != Availability.NotAvailable).ToList();
            var options = carPack.Equipment.OfType<Administration.CarPackOption>().Where(x => x.Availability != Availability.NotAvailable).ToList();
            var carPackExteriorColourTypes = carPack.Equipment.OfType<Administration.CarPackExteriorColourType>().Where(x => x.Availability != Availability.NotAvailable).ToList();
            var carPackUpholsteryTypes = carPack.Equipment.OfType<Administration.CarPackUpholsteryType>().Where(x => x.Availability != Availability.NotAvailable).ToList();

            var mappedAccessories = accessories.Select(accessory => _equipmentMapper.MapCarPackAccessory(accessory, groups, categories, isPreview, assetUrl)).OrderBy(x => x.SortIndex).ToList();
            var mappedOptions = options.Select(option => _equipmentMapper.MapCarPackOption(option, groups, categories, isPreview, assetUrl)).OrderBy(x => x.SortIndex).ToList();
            var mappedExteriorColourTypes = carPackExteriorColourTypes.Select(type => _equipmentMapper.MapCarPackExteriorColourType(type, groups, categories, isPreview, assetUrl)).OrderBy(x => x.SortIndex).ToList();
            var mappedUpholsteryTypes = carPackUpholsteryTypes.Select(type => _equipmentMapper.MapCarPackUpholsteryType(type, groups, categories, isPreview, assetUrl)).OrderBy(x => x.SortIndex).ToList();

            var index = 0;

            //foreach (var item in mappedAccessories.Cast<BaseObject>().Concat(
            //                     mappedExteriorColourTypes).Concat(
            //                     mappedOptions).Concat(
            //                     mappedUpholsteryTypes))
            //    item.SortIndex = index++;

            foreach (var item in mappedAccessories)
                item.SortIndex = index++;

            foreach (var item in mappedExteriorColourTypes)
                item.SortIndex = index++;

            foreach (var item in mappedOptions)
                item.SortIndex = index++;

            foreach (var item in mappedUpholsteryTypes)
                item.SortIndex = index++;

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
                OptionalGradeFeature = gradePack.GradeFeature && carPack.Availability == Availability.Optional,
                AvailableForExteriorColours = carPack.ExteriorColourApplicabilities.Where(item => !item.Cleared).Select(_colourMapper.MapExteriorColourApplicability).ToList(),
                AvailableForUpholsteries = carPack.UpholsteryApplicabilities.Where(item => !item.Cleared).Select(_colourMapper.MapUpholsteryApplicability).ToList(),
                Equipment = new CarPackEquipment {
                    Accessories = mappedAccessories,
                    Options = mappedOptions,
                    ExteriorColourTypes = mappedExteriorColourTypes,
                    UpholsteryTypes = mappedUpholsteryTypes
                }
            };

            return _baseMapper.MapTranslation(mappedCarPack, carPack.Translation, carPack.Name);
        }
    }
}