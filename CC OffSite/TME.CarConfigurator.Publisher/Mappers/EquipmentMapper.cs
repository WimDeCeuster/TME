using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.OpsWorks.Model;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Administration.Enums;
using TME.CarConfigurator.Administration.Interfaces;
using TME.CarConfigurator.Publisher.Exceptions;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Core;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.Repository.Objects.Interfaces;
using Car = TME.CarConfigurator.Administration.Car;
using CarAccessory = TME.CarConfigurator.Repository.Objects.Equipment.CarAccessory;
using CarEquipmentItem = TME.CarConfigurator.Repository.Objects.Equipment.CarEquipmentItem;
using CarOption = TME.CarConfigurator.Repository.Objects.Equipment.CarOption;
using EquipmentItem = TME.CarConfigurator.Administration.EquipmentItem;
using ExteriorColour = TME.CarConfigurator.Repository.Objects.Colours.ExteriorColour;
using CarPackAccessory = TME.CarConfigurator.Repository.Objects.Packs.CarPackAccessory;
using CarPackOption = TME.CarConfigurator.Repository.Objects.Packs.CarPackOption;
using CarPackExteriorColourType = TME.CarConfigurator.Repository.Objects.Packs.CarPackExteriorColourType;
using CarPackUpholsteryType = TME.CarConfigurator.Repository.Objects.Packs.CarPackUpholsteryType;
using TME.CarConfigurator.Repository.Objects.Packs;
using TME.CarConfigurator.Repository.Objects.Colours;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class EquipmentMapper : IEquipmentMapper
    {
        readonly ILabelMapper _labelMapper;
        readonly ILinkMapper _linkMapper;
        readonly ICategoryMapper _categoryInfoMapper;
        readonly IColourMapper _colourMapper;
        readonly IAssetSetMapper _assetSetMapper;

        public EquipmentMapper(ILabelMapper labelMapper, ILinkMapper linkMapper, ICategoryMapper categoryInfoMapper, IColourMapper colourMapper, IAssetSetMapper assetSetMapper)
        {
            if (labelMapper == null) throw new ArgumentNullException("labelMapper");
            if (linkMapper == null) throw new ArgumentNullException("linkMapper");
            if (categoryInfoMapper == null) throw new ArgumentNullException("categoryInfoMapper");
            if (colourMapper == null) throw new ArgumentNullException("colourMapper");
            if (assetSetMapper == null) throw new ArgumentNullException("assetSetMapper");

            _labelMapper = labelMapper;
            _linkMapper = linkMapper;
            _categoryInfoMapper = categoryInfoMapper;
            _colourMapper = colourMapper;
            _assetSetMapper = assetSetMapper;
        }

        public GradeAccessory MapGradeAccessory(ModelGenerationGradeAccessory generationGradeAccessory, Administration.Accessory crossModelAccessory, EquipmentCategories categories, IReadOnlyList<Car> cars, Boolean isPreview, ExteriorColourTypes exteriorColourTypes, String assetUrl)
        {
            var mappedGradeEquipmentItem = new GradeAccessory();

            var generationAccessory = generationGradeAccessory.Grade.Generation.Equipment[generationGradeAccessory.ID];

            return MapGradeEquipmentItem(mappedGradeEquipmentItem, generationGradeAccessory, generationAccessory, crossModelAccessory, categories, cars, isPreview, exteriorColourTypes, assetUrl);
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
            
            return MapGradeEquipmentItem(mappedEquipmentItem, generationGradeOption, generationOption, crossModelOption, categories, cars, isPreview, exteriorColourTypes, assetUrl);
        }

        public CarOption MapCarOption(Administration.CarOption generationCarOption, Administration.Option crossModelOption, EquipmentCategories categories, bool isPreview, IEnumerable<Car> cars, ExteriorColourTypes exteriorColourTypes, String assetUrl)
        {
            if (generationCarOption.HasParentOption && !generationCarOption.ParentOption.ShortID.HasValue)
                throw new CorruptDataException(String.Format("Please supply a ShortID for grade generationCarOption {0}", generationCarOption.ParentOption.ID));

            var generationOption = (ModelGenerationOption)generationCarOption.Car.Generation.Equipment[generationCarOption.ID];
            var mappedOption = new CarOption
            {
                SuffixOption = generationCarOption.SuffixOption,
                PostProductionOption = generationOption.PostProductionOption,
                TechnologyItem = generationOption.TechnologyItem,
                ParentOptionShortID = generationOption.HasParentOption ? generationCarOption.ParentOption.ShortID.Value : 0,
                Price = new Price
                {
                    ExcludingVat = generationCarOption.FittingPrice,
                    IncludingVat = generationCarOption.FittingVatPrice
                },
                AvailableForExteriorColours = generationCarOption.ExteriorColourApplicabilities.Where(item => !item.Cleared).Select(_colourMapper.MapExteriorColourApplicability).ToList(),
                AvailableForUpholsteries = generationCarOption.UpholsteryApplicabilities.Where(item => !item.Cleared).Select(_colourMapper.MapUpholsteryApplicability).ToList()
            };

            return MapCarEquipmentItem(mappedOption, generationCarOption, generationOption, crossModelOption, categories, isPreview, exteriorColourTypes, assetUrl);
        }

        public CarAccessory MapCarAccessory(Administration.CarAccessory carAccessory, Administration.Accessory crossModelAccessory, EquipmentCategories categories, bool isPreview, IEnumerable<Car> cars, ExteriorColourTypes exteriorColourTypes, String assetUrl)
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

            return MapCarEquipmentItem(mappedAccessory, carAccessory, generationAccessory, crossModelAccessory, categories, isPreview, exteriorColourTypes, assetUrl);
        }

        public CarPackAccessory MapCarPackAccessory(Administration.CarPackAccessory accessory, EquipmentGroups groups, EquipmentCategories categories, bool isPreview, IEnumerable<Car> cars, ExteriorColourTypes exteriorColourTypes, string assetUrl)
        {
            return MapCarPackEquipmentItem(new CarPackAccessory(), accessory, groups, categories, isPreview, cars, exteriorColourTypes, assetUrl);
        }

        public CarPackOption MapCarPackOption(Administration.CarPackOption option, EquipmentGroups groups, EquipmentCategories categories, bool isPreview, IEnumerable<Car> cars, ExteriorColourTypes exteriorColourTypes, string assetUrl)
        {
            
            var generationOption = (ModelGenerationOption)option.Pack.Car.Generation.Equipment[option.ID];

            var mappedOption = new CarPackOption
            {
                TechnologyItem = generationOption.TechnologyItem,
                PostProductionOption = generationOption.PostProductionOption,
                SuffixOption = generationOption.SuffixOption,
                ParentOption = GetParentOptionInfo(option.ParentOption)
            };

            return MapCarPackEquipmentItem(mappedOption, option, groups, categories, isPreview, cars, exteriorColourTypes, assetUrl);
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

        public CarPackExteriorColourType MapCarPackExteriorColourType(Administration.CarPackExteriorColourType type, EquipmentGroups groups, EquipmentCategories categories, bool isPreview, IEnumerable<Car> cars, ExteriorColourTypes exteriorColourTypes, string assetUrl)
        {
            var upholsteries = type.Pack.Car.ColourCombinations.Where(cc => cc.Approved).Select(cc => cc.Upholstery).Distinct().ToList();
            
            var mappedType = new CarPackExteriorColourType
            {
                ColourCombinations = type.ExteriorColours.Where(colour => colour.Approved).SelectMany(exteriorColour => upholsteries.Select(upholstery => new ColourCombinationInfo {
                    ExteriorColour = _colourMapper.MapExteriorColourInfo(exteriorColour),
                    Upholstery = _colourMapper.MapUpholsteryInfo(upholstery.GetInfo())
                })).ToList()
            };

            return MapCarPackEquipmentItem(mappedType, type, groups, categories, isPreview, cars, exteriorColourTypes, assetUrl);
        }

        public CarPackUpholsteryType MapCarPackUpholsteryType(Administration.CarPackUpholsteryType type, EquipmentGroups groups, EquipmentCategories categories, bool isPreview, IEnumerable<Car> cars, ExteriorColourTypes exteriorColourTypes, string assetUrl)
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

            return MapCarPackEquipmentItem(mappedType, type, groups, categories, isPreview, cars, exteriorColourTypes, assetUrl);
        }

        private T MapCarPackEquipmentItem<T>(T mappedCarPackEquipmentItem, Administration.CarPackItem carPackItem, EquipmentGroups groups, EquipmentCategories categories, bool isPreview, IEnumerable<Car> cars, ExteriorColourTypes exteriorColourTypes, string assetUrl)
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

            MapCarEquipmentItem(mappedCarPackEquipmentItem, carEquipmentItem, generationEquipmentItem, crossModelEquipmentItem, categories, isPreview, exteriorColourTypes, assetUrl);

            mappedCarPackEquipmentItem.Optional = carPackItem.Availability == Availability.Optional;
            mappedCarPackEquipmentItem.Standard = carPackItem.Availability == Availability.Standard;
            mappedCarPackEquipmentItem.SortIndex = carPackItem.Index;

            return mappedCarPackEquipmentItem;
        }

        T MapCarEquipmentItem<T>(T mappedEquipmentItem, Administration.CarEquipmentItem carEquipmentItem, ModelGenerationEquipmentItem generationEquipmentItem, EquipmentItem crossModelEquipmentItem, EquipmentCategories categories, bool isPreview, ExteriorColourTypes exteriorColourTypes, String assetUrl)
            where T : CarEquipmentItem
        {
            if (!carEquipmentItem.ShortID.HasValue)
                throw new CorruptDataException(String.Format("Please supply a ShortID for grade equipment item {0}", carEquipmentItem.ID));

            var hasColour = carEquipmentItem.Colour.ID != Guid.Empty;
            var isOwner = carEquipmentItem.Owner == MyContext.GetContext().CountryCode;

            mappedEquipmentItem = MapFromGenerationEquipmentItem(mappedEquipmentItem, generationEquipmentItem, hasColour, isPreview, exteriorColourTypes, assetUrl);
            
            //CarEquipment specific mapping
            mappedEquipmentItem.Category = _categoryInfoMapper.MapEquipmentCategoryInfo(carEquipmentItem.Category, categories);
            mappedEquipmentItem.Description = carEquipmentItem.Translation.Description;
            mappedEquipmentItem.FootNote = carEquipmentItem.Translation.FootNote;
            mappedEquipmentItem.ID = carEquipmentItem.ID;
            mappedEquipmentItem.Name = carEquipmentItem.Translation.Name.DefaultIfEmpty(carEquipmentItem.Name);
            mappedEquipmentItem.PartNumber = carEquipmentItem.PartNumber;
            mappedEquipmentItem.Path = MyContext.GetContext().EquipmentGroups.Find(carEquipmentItem.Group.ID).Path.ToLowerInvariant();
            mappedEquipmentItem.SortIndex = carEquipmentItem.Index;
            mappedEquipmentItem.ShortID = carEquipmentItem.ShortID.Value;
            mappedEquipmentItem.ToolTip = carEquipmentItem.Translation.ToolTip;
            mappedEquipmentItem.GradeFeature = carEquipmentItem.GradeFeature;
            mappedEquipmentItem.OptionalGradeFeature = carEquipmentItem.OptionalGradeFeature;
            mappedEquipmentItem.LocalCode = carEquipmentItem.LocalCode.DefaultIfEmpty(isOwner ? generationEquipmentItem.BaseCode : String.Empty);

            mappedEquipmentItem.VisibleIn = _assetSetMapper.GetVisibility(generationEquipmentItem.AssetSet).ToList();

            if (generationEquipmentItem is ModelGenerationOption && ((ModelGenerationOption)generationEquipmentItem).Components.Count != 0)
                AddComponentAssetsToVisibleIn(mappedEquipmentItem, carEquipmentItem, generationEquipmentItem);
            
            mappedEquipmentItem.Optional = carEquipmentItem.Availability == Availability.Optional;
            mappedEquipmentItem.Standard = carEquipmentItem.Availability == Availability.Standard;
            
            mappedEquipmentItem.Links = crossModelEquipmentItem == null ? new List<Repository.Objects.Link>() :
                crossModelEquipmentItem.Links.Select(link => _linkMapper.MapLink(link, isPreview)).ToList();

            mappedEquipmentItem.Labels = crossModelEquipmentItem == null ? _labelMapper.MapLabels(generationEquipmentItem.Translation.Labels) : _labelMapper.MapLabels(generationEquipmentItem.Translation.Labels, crossModelEquipmentItem.Translation.Labels);


            return mappedEquipmentItem;
        }

        T MapGradeEquipmentItem<T>(T mappedEquipmentItem, ModelGenerationGradeEquipmentItem generationGradeEquipmentItem, ModelGenerationEquipmentItem generationEquipmentItem, EquipmentItem crossModelEquipmentItem, EquipmentCategories categories, IReadOnlyList<Car> cars, Boolean isPreview, ExteriorColourTypes exteriorColourTypes, String assetUrl)
            where T : GradeEquipmentItem, IAvailabilityProperties
        {
            if (!generationGradeEquipmentItem.ShortID.HasValue)
                throw new CorruptDataException(String.Format("Please supply a ShortID for grade equipment item {0}", generationGradeEquipmentItem.Name));

            var hasColour = generationGradeEquipmentItem.Colour.ID != Guid.Empty;
            var isOwner = generationGradeEquipmentItem.Owner == MyContext.GetContext().CountryCode;

            mappedEquipmentItem = MapFromGenerationEquipmentItem(mappedEquipmentItem, generationEquipmentItem, hasColour, isPreview, exteriorColourTypes, assetUrl);
            
            //GradeEquipment specific mapping.
            mappedEquipmentItem.Category = _categoryInfoMapper.MapEquipmentCategoryInfo(generationGradeEquipmentItem.Category, categories); // ??
            mappedEquipmentItem.Description = generationGradeEquipmentItem.Translation.Description;
            mappedEquipmentItem.FootNote = generationGradeEquipmentItem.Translation.FootNote;
            mappedEquipmentItem.GradeFeature = generationGradeEquipmentItem.GradeFeature;
            mappedEquipmentItem.ID = generationGradeEquipmentItem.ID;

            mappedEquipmentItem.Labels = _labelMapper.MapLabels(generationEquipmentItem.Translation.Labels, crossModelEquipmentItem.Translation.Labels);

            mappedEquipmentItem.Links = crossModelEquipmentItem.Links.Select(link => _linkMapper.MapLink(link, isPreview)).ToList();
            mappedEquipmentItem.Name = generationGradeEquipmentItem.Translation.Name.DefaultIfEmpty(generationGradeEquipmentItem.Name);
            mappedEquipmentItem.NotAvailableOn = GetAvailabilityInfo(generationGradeEquipmentItem.ID, Availability.NotAvailable, cars);
            mappedEquipmentItem.OptionalGradeFeature = generationGradeEquipmentItem.OptionalGradeFeature;
            mappedEquipmentItem.OptionalOn = GetAvailabilityInfo(generationGradeEquipmentItem.ID, Availability.Optional, cars);
            mappedEquipmentItem.PartNumber = generationGradeEquipmentItem.PartNumber;
            mappedEquipmentItem.Path = MyContext.GetContext().EquipmentGroups.Find(generationGradeEquipmentItem.Group.ID).Path.ToLowerInvariant();
            mappedEquipmentItem.ShortID = generationGradeEquipmentItem.ShortID.Value;
            mappedEquipmentItem.SortIndex = generationGradeEquipmentItem.Index;
            mappedEquipmentItem.StandardOn = GetAvailabilityInfo(generationGradeEquipmentItem.ID, Availability.Standard, cars);
            mappedEquipmentItem.ToolTip = generationGradeEquipmentItem.Translation.ToolTip;
            mappedEquipmentItem.LocalCode = generationGradeEquipmentItem.LocalCode.DefaultIfEmpty(isOwner ? generationEquipmentItem.BaseCode : String.Empty);

            mappedEquipmentItem.NotAvailable = mappedEquipmentItem.CalculateNotAvailable();
            mappedEquipmentItem.Optional = mappedEquipmentItem.CalculateOptional();
            mappedEquipmentItem.Standard = mappedEquipmentItem.CalculateStandard();

            return mappedEquipmentItem;
        }

        private T MapFromGenerationEquipmentItem<T>(T mappedEquipmentItem, ModelGenerationEquipmentItem generationEquipmentItem, Boolean hasColour, Boolean isPreview, ExteriorColourTypes exteriorColourTypes, String assetUrl)
            where T : Repository.Objects.Equipment.EquipmentItem
        {
            mappedEquipmentItem.BestVisibleIn = new BestVisibleIn { Angle = generationEquipmentItem.BestVisibleInAngle, Mode = generationEquipmentItem.BestVisibleInMode, View = generationEquipmentItem.BestVisibleInView };
            mappedEquipmentItem.ExteriorColour = hasColour ? GetColour(generationEquipmentItem, isPreview, exteriorColourTypes, assetUrl) : null;
            mappedEquipmentItem.InternalName = generationEquipmentItem.BaseName;
            mappedEquipmentItem.KeyFeature = generationEquipmentItem.KeyFeature;
            mappedEquipmentItem.Visibility = generationEquipmentItem.Visibility.Convert();
            mappedEquipmentItem.InternalCode = generationEquipmentItem.BaseCode;

            return mappedEquipmentItem;
        }

        ExteriorColour GetColour(ModelGenerationEquipmentItem generationEquipmentItem, bool isPreview, ExteriorColourTypes exteriorColourTypes, String assetUrl)
        {
            return GetMappedExteriorColour(generationEquipmentItem, isPreview, exteriorColourTypes, assetUrl);
        }

        private ExteriorColour GetMappedExteriorColour(ModelGenerationEquipmentItem generationEquipmentItem, bool isPreview, ExteriorColourTypes exteriorColourTypes, String assetUrl)
        {
            Administration.ExteriorColourType exteriorColourType;
            var generationExteriorColour = generationEquipmentItem.Generation.ColourCombinations.ExteriorColours().FirstOrDefault(clr => clr.ID == generationEquipmentItem.Colour.ID);

            if (generationExteriorColour != null)
            {
                exteriorColourType = exteriorColourTypes[generationExteriorColour.Type.ID];
                return _colourMapper.MapExteriorColour(generationEquipmentItem.Generation, generationExteriorColour, isPreview, exteriorColourType, assetUrl);
            }

            var crossModelColour = ExteriorColours.GetExteriorColours()[generationEquipmentItem.Colour.ID];
            exteriorColourType = exteriorColourTypes[crossModelColour.Type.ID];
            return _colourMapper.MapExteriorColour(generationEquipmentItem.Generation, crossModelColour, isPreview, exteriorColourType, assetUrl);
        }

        private void AddComponentAssetsToVisibleIn<T>(T mappedEquipmentItem, Administration.CarEquipmentItem generationCarEquipmentItem,
            ModelGenerationEquipmentItem generationEquipmentItem) where T : CarEquipmentItem
        {
            var applicableComponentAssets = ((ModelGenerationOption) generationEquipmentItem).Components.GetFilteredAssets(generationCarEquipmentItem.Car);
            mappedEquipmentItem.VisibleIn.AddRange(_assetSetMapper.GetVisibility(applicableComponentAssets));
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
