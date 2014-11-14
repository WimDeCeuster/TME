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
using ExteriorColour = TME.CarConfigurator.Repository.Objects.Equipment.ExteriorColour;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class EquipmentMapper : IEquipmentMapper
    {
        readonly ILabelMapper _labelMapper;
        readonly ILinkMapper _linkMapper;
        readonly IVisibilityMapper _visibilityMapper;
        readonly ICategoryMapper _categoryInfoMapper;
        readonly IColourMapper _colourMapper;

        public EquipmentMapper(ILabelMapper labelMapper, ILinkMapper linkMapper, IVisibilityMapper visibilityMapper, ICategoryMapper categoryInfoMapper, IColourMapper colourMapper)
        {
            if (labelMapper == null) throw new ArgumentNullException("labelMapper");
            if (linkMapper == null) throw new ArgumentNullException("linkMapper");
            if (visibilityMapper == null) throw new ArgumentNullException("visibilityMapper");
            if (categoryInfoMapper == null) throw new ArgumentNullException("categoryInfoMapper");
            if (colourMapper == null) throw new ArgumentNullException("colourMapper");

            _labelMapper = labelMapper;
            _linkMapper = linkMapper;
            _visibilityMapper = visibilityMapper;
            _categoryInfoMapper = categoryInfoMapper;
            _colourMapper = colourMapper;
        }

        public GradeAccessory MapGradeAccessory(ModelGenerationGradeAccessory generationGradeAccessory, EquipmentItem crossModelAccessory, EquipmentCategories categories, IReadOnlyList<Car> cars, Boolean isPreview)
        {
            var mappedGradeEquipmentItem = new GradeAccessory();

            var generationAccessory = generationGradeAccessory.Grade.Generation.Equipment[generationGradeAccessory.ID];

            return MapGradeEquipmentItem(mappedGradeEquipmentItem, generationGradeAccessory, generationAccessory, crossModelAccessory, categories, cars, isPreview);
        }

        public GradeOption MapGradeOption(ModelGenerationGradeOption generationGradeOption, EquipmentItem crossModelOption, EquipmentCategories categories, IReadOnlyList<Car> cars, bool isPreview)
        {
            if (generationGradeOption.HasParentOption && !generationGradeOption.ParentOption.ShortID.HasValue)
                throw new CorruptDataException(String.Format("Please supply a ShortID for grade generationCarOption {0}", generationGradeOption.ParentOption.ID));

            var generationOption = (ModelGenerationOption)generationGradeOption.Grade.Generation.Equipment[generationGradeOption.ID];

            var mappedEquipmentItem = new GradeOption
            {
                TechnologyItem = generationOption.TechnologyItem,
                ParentOptionShortID = generationOption.HasParentOption ? generationGradeOption.ParentOption.ShortID.Value : 0
            };
            
            return MapGradeEquipmentItem(mappedEquipmentItem, generationGradeOption, generationOption, crossModelOption, categories, cars, isPreview);
        }

        public CarOption MapCarOption(Administration.CarOption generationCarOption, EquipmentItem crossModelEquipmentItem, EquipmentCategories categories, Boolean isPreview)
        {
            if (generationCarOption.HasParentOption && !generationCarOption.ParentOption.ShortID.HasValue)
                throw new CorruptDataException(String.Format("Please supply a ShortID for grade generationCarOption {0}", generationCarOption.ParentOption.ID));

            var generationOption = (ModelGenerationOption)generationCarOption.Car.Generation.Equipment[generationCarOption.ID];
            var mappedOption = new CarOption
            {
                TechnologyItem = generationOption.TechnologyItem,
                Price = new Price
                {
                    ExcludingVat = generationCarOption.FittingPrice,
                    IncludingVat = generationCarOption.FittingVatPrice
                }
            };

            return MapCarEquipmentItem(mappedOption, generationCarOption, generationOption, crossModelEquipmentItem,
                categories, isPreview);
        }

        public CarAccessory MapCarAccessory(Administration.CarAccessory generationCarAccessory, EquipmentItem crossModelEquipmentItem, EquipmentCategories categories, bool isPreview)
        {
            var generationAccessory = generationCarAccessory.Car.Generation.Equipment[generationCarAccessory.ID];

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
                }
            };

            return MapCarEquipmentItem(mappedAccessory, generationCarAccessory, generationAccessory, crossModelEquipmentItem, categories, isPreview);
        }

        T MapCarEquipmentItem<T>(T mappedEquipmentItem, Administration.CarEquipmentItem generationCarEquipmentItem, ModelGenerationEquipmentItem generationEquipmentItem, EquipmentItem crossModelEquipmentItem, EquipmentCategories categories, Boolean isPreview)
            where T : CarEquipmentItem
        {
            if (!generationCarEquipmentItem.ShortID.HasValue)
                throw new CorruptDataException(String.Format("Please supply a ShortID for grade equipment item {0}", generationCarEquipmentItem.ID));

            var hasColour = generationCarEquipmentItem.Colour.ID != Guid.Empty;
            var isOwner = generationCarEquipmentItem.Owner == MyContext.GetContext().CountryCode;

            mappedEquipmentItem = (T)MapFromGenerationEquipmentItem(mappedEquipmentItem, generationEquipmentItem, hasColour, isPreview);

            //CarEquipment specific mapping
            mappedEquipmentItem.Category = _categoryInfoMapper.MapEquipmentCategoryInfo(generationCarEquipmentItem.Category,categories);
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
            mappedEquipmentItem.LocalCode = generationCarEquipmentItem.LocalCode.DefaultIfEmpty(isOwner ? generationEquipmentItem.BaseCode : String.Empty);
            mappedEquipmentItem.GradeFeature = false; //?
            mappedEquipmentItem.OptionalGradeFeature = false; //?
            mappedEquipmentItem.Optional = false; //?
            mappedEquipmentItem.Standard = false; //?

            mappedEquipmentItem.Links =
                crossModelEquipmentItem.Links.Select(link => _linkMapper.MapLink(link, isPreview)).ToList();
            
            mappedEquipmentItem.Labels = _labelMapper.MapLabels(generationCarEquipmentItem.Translation.Labels,
                generationEquipmentItem.Translation.Labels, crossModelEquipmentItem.Translation.Labels);

            return mappedEquipmentItem;
        }

        T MapGradeEquipmentItem<T>(T mappedEquipmentItem, ModelGenerationGradeEquipmentItem generationGradeEquipmentItem, ModelGenerationEquipmentItem generationEquipmentItem, EquipmentItem crossModelEquipmentItem, EquipmentCategories categories, IReadOnlyList<Car> cars, Boolean isPreview)
            where T : GradeEquipmentItem, IAvailabilityProperties
        {
            if (!generationGradeEquipmentItem.ShortID.HasValue)
                throw new CorruptDataException(String.Format("Please supply a ShortID for grade equipment item {0}", generationGradeEquipmentItem.ID));

            var hasColour = generationGradeEquipmentItem.Colour.ID != Guid.Empty;
            var isOwner = generationGradeEquipmentItem.Owner == MyContext.GetContext().CountryCode;

            mappedEquipmentItem = (T)MapFromGenerationEquipmentItem(mappedEquipmentItem, generationEquipmentItem, hasColour, isPreview);
            
            //GradeEquipment specific mapping.
            mappedEquipmentItem.Category = _categoryInfoMapper.MapEquipmentCategoryInfo(generationGradeEquipmentItem.Category, categories); // ??
            mappedEquipmentItem.Description = generationGradeEquipmentItem.Translation.Description;
            mappedEquipmentItem.FootNote = generationGradeEquipmentItem.Translation.FootNote;
            mappedEquipmentItem.GradeFeature = generationGradeEquipmentItem.GradeFeature;
            mappedEquipmentItem.ID = generationGradeEquipmentItem.ID;

            mappedEquipmentItem.Labels = _labelMapper.MapLabels(generationGradeEquipmentItem.Translation.Labels, generationEquipmentItem.Translation.Labels, crossModelEquipmentItem.Translation.Labels);

            mappedEquipmentItem.Links = crossModelEquipmentItem.Links.Select(link => _linkMapper.MapLink(link, isPreview)).ToList();
            mappedEquipmentItem.Name = generationGradeEquipmentItem.Translation.Name.DefaultIfEmpty(generationGradeEquipmentItem.Name);
            mappedEquipmentItem.NotAvailableOn = GetAvailabilityInfo(generationGradeEquipmentItem, Availability.NotAvailable, cars);
            mappedEquipmentItem.OptionalGradeFeature = generationGradeEquipmentItem.OptionalGradeFeature;
            mappedEquipmentItem.OptionalOn = GetAvailabilityInfo(generationGradeEquipmentItem, Availability.Optional, cars);
            mappedEquipmentItem.PartNumber = generationGradeEquipmentItem.PartNumber;
            mappedEquipmentItem.Path = MyContext.GetContext().EquipmentGroups.Find(generationGradeEquipmentItem.Group.ID).Path.ToLowerInvariant();
            mappedEquipmentItem.ShortID = generationGradeEquipmentItem.ShortID.Value;
            mappedEquipmentItem.SortIndex = generationGradeEquipmentItem.Index;
            mappedEquipmentItem.StandardOn = GetAvailabilityInfo(generationGradeEquipmentItem, Availability.Standard, cars);
            mappedEquipmentItem.ToolTip = generationGradeEquipmentItem.Translation.ToolTip;
            mappedEquipmentItem.LocalCode = generationGradeEquipmentItem.LocalCode.DefaultIfEmpty(isOwner ? generationEquipmentItem.BaseCode : String.Empty);

            mappedEquipmentItem.NotAvailable = mappedEquipmentItem.CalculateNotAvailable();
            mappedEquipmentItem.Optional = mappedEquipmentItem.CalculateOptional();
            mappedEquipmentItem.Standard = mappedEquipmentItem.CalculateStandard();

            return mappedEquipmentItem;
        }

        private Repository.Objects.Equipment.EquipmentItem MapFromGenerationEquipmentItem(Repository.Objects.Equipment.EquipmentItem mappedEquipmentItem, ModelGenerationEquipmentItem generationEquipmentItem, Boolean hasColour, Boolean isPreview)
        {
            mappedEquipmentItem.BestVisibleIn = new BestVisibleIn { Angle = generationEquipmentItem.BestVisibleInAngle, Mode = generationEquipmentItem.BestVisibleInMode, View = generationEquipmentItem.BestVisibleInView };
            mappedEquipmentItem.ExteriorColour = hasColour ? GetColour(generationEquipmentItem, isPreview) : null;
            mappedEquipmentItem.InternalName = generationEquipmentItem.BaseName;
            mappedEquipmentItem.KeyFeature = generationEquipmentItem.KeyFeature;
            mappedEquipmentItem.Visibility = _visibilityMapper.MapVisibility(generationEquipmentItem.Visibility);
            mappedEquipmentItem.InternalCode = generationEquipmentItem.BaseCode;

            return mappedEquipmentItem;
        }

        ExteriorColour GetColour(ModelGenerationEquipmentItem generationEquipmentItem, bool isPreview)
        {
            var mappedExteriorColour = GetMappedExteriorColour(generationEquipmentItem, isPreview);

            return new ExteriorColour()
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

        private Repository.Objects.Colours.ExteriorColour GetMappedExteriorColour(ModelGenerationEquipmentItem generationEquipmentItem, bool isPreview)
        {
            var generationExteriorColour = generationEquipmentItem.Generation.ColourCombinations.ExteriorColours()
                    .FirstOrDefault(clr => clr.ID == generationEquipmentItem.Colour.ID);

            if (generationExteriorColour != null)
                return _colourMapper.MapExteriorColour(generationEquipmentItem.Generation, generationExteriorColour, isPreview);

            var crossModelColour = ExteriorColours.GetExteriorColours()[generationEquipmentItem.Colour.ID];
            return _colourMapper.MapExteriorColour(generationEquipmentItem.Generation, crossModelColour, isPreview);
        }

        static IReadOnlyList<CarInfo> GetAvailabilityInfo(ModelGenerationGradeEquipmentItem generationGradeEquipmentItem, Availability availability, IEnumerable<Car> cars)
        {
            return cars.Where(car => car.Equipment[generationGradeEquipmentItem.ID] != null ? car.Equipment[generationGradeEquipmentItem.ID].Availability == availability : availability == Availability.NotAvailable)
                       .Select(car => new CarInfo
                       {
                           Name = car.Translation.Name.DefaultIfEmpty(car.Name),
                           ShortID = car.ShortID.Value
                       })
                       .ToList();
        }

    }
}
