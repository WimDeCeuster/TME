using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Administration.Enums;
using TME.CarConfigurator.Publisher.Exceptions;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.Repository.Objects.Core;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.Repository.Objects.Interfaces;
using Car = TME.CarConfigurator.Administration.Car;
using CarAccessory = TME.CarConfigurator.Repository.Objects.Equipment.CarAccessory;
using CarEquipmentItem = TME.CarConfigurator.Repository.Objects.Equipment.CarEquipmentItem;
using CarOption = TME.CarConfigurator.Repository.Objects.Equipment.CarOption;
using EquipmentItem = TME.CarConfigurator.Administration.EquipmentItem;
using CarPackAccessory = TME.CarConfigurator.Repository.Objects.Packs.CarPackAccessory;
using CarPackOption = TME.CarConfigurator.Repository.Objects.Packs.CarPackOption;
using CarPackExteriorColourType = TME.CarConfigurator.Repository.Objects.Packs.CarPackExteriorColourType;
using CarPackUpholsteryType = TME.CarConfigurator.Repository.Objects.Packs.CarPackUpholsteryType;
using TME.CarConfigurator.Repository.Objects.Packs;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class EquipmentMapper : IEquipmentMapper
    {
        readonly ILabelMapper _labelMapper;
        readonly ILinkMapper _linkMapper;
        readonly ICategoryMapper _categoryInfoMapper;
        readonly IColourMapper _colourMapper;
        readonly IAssetSetMapper _assetSetMapper;
        readonly IBaseMapper _baseMapper;

        public EquipmentMapper(ILabelMapper labelMapper, ILinkMapper linkMapper, ICategoryMapper categoryInfoMapper, IColourMapper colourMapper, IAssetSetMapper assetSetMapper, IBaseMapper baseMapper)
        {
            if (labelMapper == null) throw new ArgumentNullException("labelMapper");
            if (linkMapper == null) throw new ArgumentNullException("linkMapper");
            if (categoryInfoMapper == null) throw new ArgumentNullException("categoryInfoMapper");
            if (colourMapper == null) throw new ArgumentNullException("colourMapper");
            if (assetSetMapper == null) throw new ArgumentNullException("assetSetMapper");
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");

            _labelMapper = labelMapper;
            _linkMapper = linkMapper;
            _categoryInfoMapper = categoryInfoMapper;
            _colourMapper = colourMapper;
            _assetSetMapper = assetSetMapper;
            _baseMapper = baseMapper;
        }

        public GradeAccessory MapGradeAccessory(ModelGenerationGradeAccessory generationGradeAccessory, Administration.Accessory crossModelAccessory, EquipmentCategories categories, IReadOnlyList<Car> cars, Boolean isPreview, ExteriorColourTypes exteriorColourTypes, String assetUrl)
        {
            var mappedGradeEquipmentItem = new GradeAccessory();

            var generationAccessory = generationGradeAccessory.Grade.Generation.Equipment[generationGradeAccessory.ID];

            return MapGradeEquipmentItem(mappedGradeEquipmentItem, generationGradeAccessory, generationAccessory, crossModelAccessory, categories, cars, isPreview, assetUrl);
        }

        public GradeOption MapGradeOption(ModelGenerationGradeOption generationGradeOption, Administration.Option crossModelOption, EquipmentCategories categories, IReadOnlyList<Car> cars, bool isPreview, ExteriorColourTypes exteriorColourTypes, String assetUrl)
        {
            if (generationGradeOption.HasParentOption && !generationGradeOption.ParentOption.ShortID.HasValue)
                throw new CorruptDataException(String.Format("Please supply a ShortID for grade option {0}", generationGradeOption.ParentOption.Name));

            var generationOption = (ModelGenerationOption)generationGradeOption.Grade.Generation.Equipment[generationGradeOption.ID];

            var mappedEquipmentItem = new GradeOption
            {
                TechnologyItem = generationOption.TechnologyItem,
                ParentOptionShortID = generationOption.HasParentOption ? generationGradeOption.ParentOption.ShortID.Value : 0
            };
            
            return MapGradeEquipmentItem(mappedEquipmentItem, generationGradeOption, generationOption, crossModelOption, categories, cars, isPreview, assetUrl);
        }

        public CarOption MapCarOption(Administration.CarOption carOption, Administration.Option crossModelOption, EquipmentCategories categories, bool isPreview, String assetUrl)
        {
            if (carOption.HasParentOption && !carOption.ParentOption.ShortID.HasValue)
                throw new CorruptDataException(String.Format("Please supply a ShortID for grade generationCarOption {0}", carOption.ParentOption.ID));
            
            var generationOption = (ModelGenerationOption)carOption.Car.Generation.Equipment[carOption.ID];
            var mappedOption = new CarOption
            {
                SuffixOption = carOption.SuffixOption,
                PostProductionOption = generationOption.PostProductionOption,
                TechnologyItem = generationOption.TechnologyItem,
                ParentOptionShortID = generationOption.HasParentOption ? carOption.ParentOption.ShortID.Value : 0,
                Price = new Price
                {
                    ExcludingVat = carOption.FittingPrice,
                    IncludingVat = carOption.FittingVatPrice
                },
                AvailableForExteriorColours = carOption.ExteriorColourApplicabilities.Where(item => !item.Cleared).Select(_colourMapper.MapExteriorColourApplicability).ToList(),
                AvailableForUpholsteries = carOption.UpholsteryApplicabilities.Where(item => !item.Cleared).Select(_colourMapper.MapUpholsteryApplicability).ToList()
            };

            return MapCarEquipmentItem(mappedOption, carOption, generationOption, crossModelOption, categories, isPreview, assetUrl, true);
        }

        public CarAccessory MapCarAccessory(Administration.CarAccessory carAccessory, Administration.Accessory crossModelAccessory, EquipmentCategories categories, bool isPreview, String assetUrl)
        {
            var generationAccessory = carAccessory.Car.Generation.Equipment[carAccessory.ID];
            
            var colourCombinations = carAccessory.Car.ColourCombinations;
            var approvedColourCombinations = colourCombinations.Where(cc => cc.Approved).ToList();
            var exteriorColours = approvedColourCombinations.Select(cc => cc.ExteriorColour).Distinct().ToList();
            var upholsteries = approvedColourCombinations.Select(cc => cc.Upholstery).Distinct().ToList();

            var mappedAccessory = new CarAccessory
            {
                BasePrice = new Price
                {
                    ExcludingVat = carAccessory.BasePrice,
                    IncludingVat = carAccessory.BaseVatPrice
                },

                MountingCostsOnNewVehicle = new MountingCosts
                {
                    Price = new Price
                    {
                        ExcludingVat = carAccessory.FittingPriceNewCar,
                        IncludingVat = carAccessory.FittingVatPriceNewCar
                    },
                    Time = carAccessory.FittingTimeNewCar
                },

                MountingCostsOnUsedVehicle = new MountingCosts
                {
                    Price = new Price
                    {
                        ExcludingVat = carAccessory.FittingPriceExistingCar,
                        IncludingVat = carAccessory.FittingVatPriceExistingCar
                    },
                    Time = carAccessory.FittingTimeExistingCar
                },

                AvailableForExteriorColours = crossModelAccessory.ExteriorColourApplicabilities.Where(item => exteriorColours.Any(colour => colour.ID == item.ID)).Select(_colourMapper.MapExteriorColourApplicability).ToList(),
                AvailableForUpholsteries = upholsteries.Where(upholstery => crossModelAccessory.InteriorColourApplicabilities.Any(item => upholstery.InteriorColour.ID == item.ID)).Select(upholstery => _colourMapper.MapUpholsteryInfo(upholstery.GetInfo())).ToList()
            };

            return MapCarEquipmentItem(mappedAccessory, carAccessory, generationAccessory, crossModelAccessory, categories, isPreview, assetUrl, true);
        }

        public CarPackAccessory MapCarPackAccessory(Administration.CarPackAccessory accessory, EquipmentGroups groups, EquipmentCategories categories, bool isPreview, string assetUrl)
        {
            var crossModelAccessory = groups.FindAccessory(accessory.ID);
            var colourCombinations = accessory.Pack.Car.ColourCombinations;
            var approvedColourCombinations = colourCombinations.Where(cc => cc.Approved).ToList();
            var exteriorColours = approvedColourCombinations.Select(cc => cc.ExteriorColour).Distinct().ToList();
            var upholsteries = approvedColourCombinations.Select(cc => cc.Upholstery).Distinct().ToList();

            var mappedAccessory = new CarPackAccessory
            {
                AvailableForExteriorColours = crossModelAccessory.ExteriorColourApplicabilities.Where(item => exteriorColours.Any(colour => colour.ID == item.ID)).Select(_colourMapper.MapExteriorColourApplicability).ToList(),
                AvailableForUpholsteries = upholsteries.Where(upholstery => crossModelAccessory.InteriorColourApplicabilities.Any(item => upholstery.InteriorColour.ID == item.ID)).Select(upholstery => _colourMapper.MapUpholsteryInfo(upholstery.GetInfo())).ToList()
            };

            return MapCarPackEquipmentItem(mappedAccessory, accessory, groups, categories, isPreview, assetUrl, true);
        }

        public CarPackOption MapCarPackOption(Administration.CarPackOption option, EquipmentGroups groups, EquipmentCategories categories, bool isPreview, string assetUrl)
        {
            var generationOption = (ModelGenerationOption)option.Pack.Car.Generation.Equipment[option.ID];
            var carOption = (Administration.CarOption)option.Pack.Car.Equipment[option.ID];

            var mappedOption = new CarPackOption
            {
                TechnologyItem = generationOption.TechnologyItem,
                PostProductionOption = generationOption.PostProductionOption,
                SuffixOption = generationOption.SuffixOption,
                ParentOption = GetParentOptionInfo(option.ParentOption),
                AvailableForExteriorColours = carOption.ExteriorColourApplicabilities.Where(item => !item.Cleared).Select(_colourMapper.MapExteriorColourApplicability).ToList(),
                AvailableForUpholsteries = carOption.UpholsteryApplicabilities.Where(item => !item.Cleared).Select(_colourMapper.MapUpholsteryApplicability).ToList()
            };

            return MapCarPackEquipmentItem(mappedOption, option, groups, categories, isPreview, assetUrl, true);
        }

        private Repository.Objects.Equipment.OptionInfo GetParentOptionInfo(Administration.CarPackOption carPackOption)
        {
            if (carPackOption == null)
                return null;

            if (!carPackOption.ShortID.HasValue)
                throw new CorruptDataException(String.Format("Please supply a ShortID for car pack option {0}", carPackOption.ID));

            return new Repository.Objects.Equipment.OptionInfo
            {
                ShortID = carPackOption.ShortID.Value,
                Name = carPackOption.Name
            };
        }

        public CarPackExteriorColourType MapCarPackExteriorColourType(Administration.CarPackExteriorColourType type, EquipmentGroups groups, EquipmentCategories categories, bool isPreview, string assetUrl)
        {
            var upholsteries = type.Pack.Car.ColourCombinations.Where(cc => cc.Approved).Select(cc => cc.Upholstery).Distinct().ToList();
            
            var mappedType = new CarPackExteriorColourType
            {
                ColourCombinations = type.ExteriorColours.Where(colour => colour.Approved).SelectMany(exteriorColour => upholsteries.Select(upholstery => new ColourCombinationInfo {
                    ExteriorColour = _colourMapper.MapExteriorColourInfo(exteriorColour),
                    Upholstery = _colourMapper.MapUpholsteryInfo(upholstery.GetInfo())
                })).ToList()
            };

            return MapCarPackEquipmentItem(mappedType, type, groups, categories, isPreview, assetUrl, true);
        }

        public CarPackUpholsteryType MapCarPackUpholsteryType(Administration.CarPackUpholsteryType type, EquipmentGroups groups, EquipmentCategories categories, bool isPreview, string assetUrl)
        {
            var exteriorColours = type.Pack.Car.ColourCombinations.Where(cc => cc.Approved).Select(cc => cc.ExteriorColour).Distinct();

            var mappedType = new CarPackUpholsteryType
            {
                ColourCombinations = type.Upholsteries.Where(upholstery => upholstery.Approved).SelectMany(upholstery => exteriorColours.Select(exteriorColour => new ColourCombinationInfo
                {
                    ExteriorColour = _colourMapper.MapExteriorColourInfo(exteriorColour.GetInfo()),
                    Upholstery = _colourMapper.MapUpholsteryInfo(upholstery)
                })).ToList()
            };

            return MapCarPackEquipmentItem(mappedType, type, groups, categories, isPreview, assetUrl, true);
        }

        private T MapCarPackEquipmentItem<T>(T mappedCarPackEquipmentItem, CarPackItem carPackItem, EquipmentGroups groups, EquipmentCategories categories, bool isPreview, string assetUrl, bool canHaveAssets)
            where T : CarPackEquipmentItem
        {
            var carEquipmentItem = carPackItem.Pack.Car.Equipment[carPackItem.ID];
            var generationEquipmentItem = carPackItem.Pack.Car.Generation.Equipment[carPackItem.ID];
            var crossModelEquipmentItem = groups.FindEquipment(carPackItem.ID);
            
            mappedCarPackEquipmentItem.Price = new Price
            {
                ExcludingVat = carPackItem.Price,
                IncludingVat = carPackItem.VatPrice
            };
            
            mappedCarPackEquipmentItem.AvailableForExteriorColours = new List<Repository.Objects.Colours.ExteriorColourInfo>();
            mappedCarPackEquipmentItem.AvailableForUpholsteries = new List<Repository.Objects.Colours.UpholsteryInfo>();
            
            mappedCarPackEquipmentItem.ColouringModes = carPackItem.ColouringModes.Convert();

            MapCarEquipmentItem(mappedCarPackEquipmentItem, carEquipmentItem, generationEquipmentItem, crossModelEquipmentItem, categories, isPreview, assetUrl, canHaveAssets);

            mappedCarPackEquipmentItem.Optional = carPackItem.Availability == Availability.Optional;
            mappedCarPackEquipmentItem.Standard = carPackItem.Availability == Availability.Standard;
            
            return mappedCarPackEquipmentItem;
        }

        T MapCarEquipmentItem<T>(T mappedEquipmentItem, Administration.CarEquipmentItem carEquipmentItem, ModelGenerationEquipmentItem generationEquipmentItem, EquipmentItem crossModelEquipmentItem, EquipmentCategories categories, bool isPreview, String assetUrl, bool canHaveAssets)
            where T : CarEquipmentItem
        {
            if (!carEquipmentItem.ShortID.HasValue)
                throw new CorruptDataException(String.Format("Please supply a ShortID for grade equipment item {0}", carEquipmentItem.ID));


            mappedEquipmentItem = MapFromGenerationEquipmentItem(mappedEquipmentItem, generationEquipmentItem, crossModelEquipmentItem, isPreview, categories, assetUrl);
            
            mappedEquipmentItem.GradeFeature = carEquipmentItem.GradeFeature;
            mappedEquipmentItem.OptionalGradeFeature = carEquipmentItem.OptionalGradeFeature;
            
            mappedEquipmentItem.VisibleIn = _assetSetMapper.GetVisibility(generationEquipmentItem.AssetSet, canHaveAssets).ToList();

            if (generationEquipmentItem is ModelGenerationOption && ((ModelGenerationOption)generationEquipmentItem).Components.Count != 0)
                AddComponentAssetsToVisibleIn(mappedEquipmentItem, carEquipmentItem, generationEquipmentItem, canHaveAssets);
            
            mappedEquipmentItem.Optional = carEquipmentItem.Availability == Availability.Optional;
            mappedEquipmentItem.Standard = carEquipmentItem.Availability == Availability.Standard;
           
            return mappedEquipmentItem;
        }

        T MapGradeEquipmentItem<T>(T mappedEquipmentItem, ModelGenerationGradeEquipmentItem generationGradeEquipmentItem, ModelGenerationEquipmentItem generationEquipmentItem, EquipmentItem crossModelEquipmentItem, EquipmentCategories categories, IReadOnlyList<Car> cars, Boolean isPreview, String assetUrl)
            where T : GradeEquipmentItem, IAvailabilityProperties
        {
            if (!generationGradeEquipmentItem.ShortID.HasValue)
                throw new CorruptDataException(String.Format("Please supply a ShortID for grade equipment item {0}", generationGradeEquipmentItem.Name));

            mappedEquipmentItem = MapFromGenerationEquipmentItem(mappedEquipmentItem, generationEquipmentItem, crossModelEquipmentItem, isPreview, categories, assetUrl);
            
            mappedEquipmentItem.GradeFeature = generationGradeEquipmentItem.GradeFeature;
            
            mappedEquipmentItem.NotAvailableOn = GetAvailabilityInfo(generationGradeEquipmentItem.ID, Availability.NotAvailable, cars);
            mappedEquipmentItem.OptionalGradeFeature = generationGradeEquipmentItem.OptionalGradeFeature;
            mappedEquipmentItem.OptionalOn = GetAvailabilityInfo(generationGradeEquipmentItem.ID, Availability.Optional, cars);
            mappedEquipmentItem.StandardOn = GetAvailabilityInfo(generationGradeEquipmentItem.ID, Availability.Standard, cars);
            
            mappedEquipmentItem.NotAvailable = mappedEquipmentItem.CalculateNotAvailable();
            mappedEquipmentItem.Optional = mappedEquipmentItem.CalculateOptional();
            mappedEquipmentItem.Standard = mappedEquipmentItem.CalculateStandard();

            return mappedEquipmentItem;
        }

        private T MapFromGenerationEquipmentItem<T>(T mappedEquipmentItem, ModelGenerationEquipmentItem generationEquipmentItem, EquipmentItem crossModelEquipmentItem, Boolean isPreview, EquipmentCategories categories, String assetUrl)
            where T : Repository.Objects.Equipment.EquipmentItem
        {
            _baseMapper.MapDefaults(mappedEquipmentItem, generationEquipmentItem);
            var isOwner = generationEquipmentItem.Owner == MyContext.GetContext().CountryCode;
            var hasColour = generationEquipmentItem.Colour.ID != Guid.Empty;
            mappedEquipmentItem.BestVisibleIn = new BestVisibleIn { Angle = generationEquipmentItem.BestVisibleInAngle, Mode = generationEquipmentItem.BestVisibleInMode, View = generationEquipmentItem.BestVisibleInView };
            mappedEquipmentItem.ExteriorColour = hasColour ? GetColour(generationEquipmentItem, isPreview, assetUrl) : null;
            mappedEquipmentItem.InternalName = generationEquipmentItem.BaseName;
            mappedEquipmentItem.KeyFeature = generationEquipmentItem.KeyFeature;
            mappedEquipmentItem.Visibility = generationEquipmentItem.Visibility.Convert();
            mappedEquipmentItem.InternalCode = generationEquipmentItem.BaseCode;

            mappedEquipmentItem.LocalCode = generationEquipmentItem.LocalCode.DefaultIfEmpty(isOwner ? generationEquipmentItem.BaseCode : String.Empty);
            mappedEquipmentItem.ShortID = generationEquipmentItem.ShortID.Value;
            mappedEquipmentItem.PartNumber = generationEquipmentItem.PartNumber;
            mappedEquipmentItem.SortIndex = generationEquipmentItem.Index;
            mappedEquipmentItem.Path = MyContext.GetContext().EquipmentGroups.Find(generationEquipmentItem.Group.ID).Path.ToLowerInvariant();
            mappedEquipmentItem.Category = _categoryInfoMapper.MapEquipmentCategoryInfo(generationEquipmentItem.Category, categories);
            mappedEquipmentItem.Links = crossModelEquipmentItem == null ? new List<Repository.Objects.Link>() : crossModelEquipmentItem.Links.Select(link => _linkMapper.MapLink(link, isPreview)).ToList();

            return mappedEquipmentItem;
        }

        Repository.Objects.Equipment.ExteriorColour GetColour(ModelGenerationEquipmentItem generationEquipmentItem, bool isPreview, String assetUrl)
        {
            var generationExteriorColour = generationEquipmentItem.Generation.ColourCombinations.ExteriorColours().FirstOrDefault(clr => clr.ID == generationEquipmentItem.Colour.ID);

            if (generationExteriorColour != null)
                 return _colourMapper.MapEquipmentExteriorColour(generationEquipmentItem.Generation, generationExteriorColour, isPreview, assetUrl);
           
            var crossModelColour = ExteriorColours.GetExteriorColours()[generationEquipmentItem.Colour.ID];
            return _colourMapper.MapEquipmentExteriorColour(generationEquipmentItem.Generation, crossModelColour, isPreview, assetUrl);
        }

        private void AddComponentAssetsToVisibleIn<T>(T mappedEquipmentItem, Administration.CarEquipmentItem generationCarEquipmentItem,
            ModelGenerationEquipmentItem generationEquipmentItem,bool canHaveAssets) where T : CarEquipmentItem
        {
            var applicableComponentAssets = ((ModelGenerationOption) generationEquipmentItem).Components.GetFilteredAssets(generationCarEquipmentItem.Car);
            mappedEquipmentItem.VisibleIn.AddRange(_assetSetMapper.GetVisibility(applicableComponentAssets, canHaveAssets));
            mappedEquipmentItem.VisibleIn = mappedEquipmentItem.VisibleIn.Distinct().ToList();
        }

        static IReadOnlyList<CarInfo> GetAvailabilityInfo(Guid equipmentItemID, Availability availability, IEnumerable<Car> cars)
        {
            return cars.Where(car => car.Equipment[equipmentItemID] != null ? car.Equipment[equipmentItemID].Availability == availability : availability == Availability.NotAvailable)
                       .Select(car => new CarInfo
                       {
                           Name = car.Translation.Name.DefaultIfEmpty(car.Name),
                           ShortID = car.ShortID.Value
                       })
                       .ToList();
        }
    }
}
