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
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.Repository.Objects.Interfaces;
using Car = TME.CarConfigurator.Administration.Car;
using CarAccessory = TME.CarConfigurator.Repository.Objects.Equipment.CarAccessory;
using CarEquipmentItem = TME.CarConfigurator.Repository.Objects.Equipment.CarEquipmentItem;
using CarOption = TME.CarConfigurator.Repository.Objects.Equipment.CarOption;
using EquipmentItem = TME.CarConfigurator.Administration.EquipmentItem;
using ExteriorColour = TME.CarConfigurator.Repository.Objects.Colours.ExteriorColour;


namespace TME.CarConfigurator.Publisher.Mappers
{
    public class EquipmentMapper : IEquipmentMapper
    {
        readonly ILabelMapper _labelMapper;
        readonly ILinkMapper _linkMapper;
        readonly IVisibilityMapper _visibilityMapper;
        readonly ICategoryMapper _categoryInfoMapper;
        readonly IColourMapper _colourMapper;
        readonly IAssetSetMapper _assetSetMapper;

        public EquipmentMapper(ILabelMapper labelMapper, ILinkMapper linkMapper, IVisibilityMapper visibilityMapper, ICategoryMapper categoryInfoMapper, IColourMapper colourMapper, IAssetSetMapper assetSetMapper)
        {
            if (labelMapper == null) throw new ArgumentNullException("labelMapper");
            if (linkMapper == null) throw new ArgumentNullException("linkMapper");
            if (visibilityMapper == null) throw new ArgumentNullException("visibilityMapper");
            if (categoryInfoMapper == null) throw new ArgumentNullException("categoryInfoMapper");
            if (colourMapper == null) throw new ArgumentNullException("colourMapper");
            if (assetSetMapper == null) throw new ArgumentNullException("assetSetMapper");

            _labelMapper = labelMapper;
            _linkMapper = linkMapper;
            _visibilityMapper = visibilityMapper;
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

        public CarAccessory MapCarAccessory(Administration.CarAccessory generationCarAccessory, Administration.Accessory crossModelAccessory, EquipmentCategories categories, bool isPreview, IEnumerable<Car> cars, Car car, ExteriorColourTypes exteriorColourTypes, String assetUrl)
        {
            var generationAccessory = generationCarAccessory.Car.Generation.Equipment[generationCarAccessory.ID];

            var colourCombinations = car.ColourCombinations;
            var exteriorColours = colourCombinations.ExteriorColours();
            var upholsteries = colourCombinations.Upholsteries();

            var mappedAccessory = new CarAccessory
            {
                BasePrice = new Price
                {
                    ExcludingVat = generationCarAccessory.BasePrice,
                    IncludingVat = generationCarAccessory.BaseVatPrice
                },

                MountingCostsOnNewVehicle = new MountingCosts
                {
                    Price = new Price
                    {
                        ExcludingVat = generationCarAccessory.FittingPriceNewCar,
                        IncludingVat = generationCarAccessory.FittingVatPriceNewCar
                    },
                    Time = generationCarAccessory.FittingTimeNewCar
                },

                MountingCostsOnUsedVehicle = new MountingCosts
                {
                    Price = new Price
                    {
                        ExcludingVat = generationCarAccessory.FittingPriceExistingCar,
                        IncludingVat = generationCarAccessory.FittingVatPriceExistingCar
                    },
                    Time = generationCarAccessory.FittingTimeExistingCar
                },

                AvailableForExteriorColours = crossModelAccessory.ExteriorColourApplicabilities.Where(item => exteriorColours.Any(colour => colour.ID == item.ID)).Select(_colourMapper.MapExteriorColourApplicability).ToList(),
                AvailableForUpholsteries = upholsteries.Where(upholstery => crossModelAccessory.InteriorColourApplicabilities.Any(item => upholstery.InteriorColour.ID == item.ID)).Select(_colourMapper.MapUpholsteryInfo).ToList()
            };

            return MapCarEquipmentItem(mappedAccessory, generationCarAccessory, generationAccessory, crossModelAccessory, categories, isPreview, exteriorColourTypes, assetUrl);
        }

        T MapCarEquipmentItem<T>(T mappedEquipmentItem, Administration.CarEquipmentItem generationCarEquipmentItem, ModelGenerationEquipmentItem generationEquipmentItem, EquipmentItem crossModelEquipmentItem, EquipmentCategories categories, bool isPreview, ExteriorColourTypes exteriorColourTypes, String assetUrl)
            where T : CarEquipmentItem
        {
            if (!generationCarEquipmentItem.ShortID.HasValue)
                throw new CorruptDataException(String.Format("Please supply a ShortID for grade equipment item {0}", generationCarEquipmentItem.ID));

            var hasColour = generationCarEquipmentItem.Colour.ID != Guid.Empty;
            var isOwner = generationCarEquipmentItem.Owner == MyContext.GetContext().CountryCode;

            mappedEquipmentItem = MapFromGenerationEquipmentItem(mappedEquipmentItem, generationEquipmentItem, hasColour, isPreview, exteriorColourTypes, assetUrl);
            
            //CarEquipment specific mapping
            mappedEquipmentItem.Category = _categoryInfoMapper.MapEquipmentCategoryInfo(generationCarEquipmentItem.Category, categories);
            mappedEquipmentItem.Description = generationCarEquipmentItem.Translation.Description;
            mappedEquipmentItem.FootNote = generationCarEquipmentItem.Translation.FootNote;
            mappedEquipmentItem.ID = generationCarEquipmentItem.ID;
            mappedEquipmentItem.LocalCode = generationCarEquipmentItem.LocalCode;
            mappedEquipmentItem.Name = generationCarEquipmentItem.Translation.Name.DefaultIfEmpty(generationCarEquipmentItem.Name);
            mappedEquipmentItem.PartNumber = generationCarEquipmentItem.PartNumber;
            mappedEquipmentItem.Path = MyContext.GetContext().EquipmentGroups.Find(generationCarEquipmentItem.Group.ID).Path.ToLowerInvariant();
            mappedEquipmentItem.SortIndex = generationCarEquipmentItem.Index;
            mappedEquipmentItem.ShortID = generationCarEquipmentItem.ShortID.Value;
            mappedEquipmentItem.ToolTip = generationCarEquipmentItem.Translation.ToolTip;
            mappedEquipmentItem.GradeFeature = generationCarEquipmentItem.GradeFeature;
            mappedEquipmentItem.OptionalGradeFeature = generationCarEquipmentItem.OptionalGradeFeature;
            mappedEquipmentItem.LocalCode = generationCarEquipmentItem.LocalCode.DefaultIfEmpty(isOwner ? generationEquipmentItem.BaseCode : String.Empty);
            mappedEquipmentItem.VisibleIn =
                _assetSetMapper.GetVisibility(generationEquipmentItem.AssetSet).ToList();
            
            mappedEquipmentItem.Optional = generationCarEquipmentItem.Availability == Availability.Optional;
            mappedEquipmentItem.Standard = generationCarEquipmentItem.Availability == Availability.Standard;
            
            mappedEquipmentItem.Links =
                crossModelEquipmentItem.Links.Select(link => _linkMapper.MapLink(link, isPreview)).ToList();

            mappedEquipmentItem.Labels = _labelMapper.MapLabels(generationEquipmentItem.Translation.Labels, crossModelEquipmentItem.Translation.Labels);


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
            mappedEquipmentItem.Visibility = _visibilityMapper.MapVisibility(generationEquipmentItem.Visibility);
            mappedEquipmentItem.InternalCode = generationEquipmentItem.BaseCode;

            return mappedEquipmentItem;
        }

        ExteriorColour GetColour(ModelGenerationEquipmentItem generationEquipmentItem, bool isPreview, ExteriorColourTypes exteriorColourTypes, String assetUrl)
        {
            var mappedExteriorColour = GetMappedExteriorColour(generationEquipmentItem, isPreview, exteriorColourTypes, assetUrl);

            return new ExteriorColour
            {
                ID = mappedExteriorColour.ID,
                InternalCode = mappedExteriorColour.InternalCode,
                LocalCode = mappedExteriorColour.LocalCode,
                Name = mappedExteriorColour.Name,
                Description = mappedExteriorColour.Description,
                FootNote = mappedExteriorColour.FootNote,
                ToolTip = mappedExteriorColour.ToolTip,
                Labels = mappedExteriorColour.Labels,
                SortIndex = mappedExteriorColour.SortIndex,
                Transformation = mappedExteriorColour.Transformation
            };
        }

        private ExteriorColour GetMappedExteriorColour(ModelGenerationEquipmentItem generationEquipmentItem, bool isPreview, ExteriorColourTypes exteriorColourTypes, String assetUrl)
        {
            ExteriorColourType exteriorColourType;
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
