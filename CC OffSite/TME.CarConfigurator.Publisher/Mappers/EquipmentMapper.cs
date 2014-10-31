using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Administration.Enums;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Exceptions;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Equipment;
using Car = TME.CarConfigurator.Administration.Car;
using ExteriorColour = TME.CarConfigurator.Repository.Objects.Colours.ExteriorColour;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class EquipmentMapper : IEquipmentMapper
    {
        IBaseMapper _baseMapper;
        readonly ILabelMapper _labelMapper;
        readonly ILinkMapper _linkMapper;
        readonly IVisibilityMapper _visibilityMapper;
        readonly ICategoryInfoMapper _categoryInfoMapper;
        readonly IColourMapper _colourMapper;

        public EquipmentMapper(IBaseMapper baseMapper, ILabelMapper labelMapper, ILinkMapper linkMapper, IVisibilityMapper visibilityMapper, ICategoryInfoMapper categoryInfoMapper, IColourMapper colourMapper)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");
            if (labelMapper == null) throw new ArgumentNullException("labelMapper");
            if (linkMapper == null) throw new ArgumentNullException("linkMapper");
            if (visibilityMapper == null) throw new ArgumentNullException("visibilityMapper");
            if (categoryInfoMapper == null) throw new ArgumentNullException("categoryInfoMapper");
            if (colourMapper == null) throw new ArgumentNullException("colourMapper");

            _baseMapper = baseMapper;
            _labelMapper = labelMapper;
            _linkMapper = linkMapper;
            _visibilityMapper = visibilityMapper;
            _categoryInfoMapper = categoryInfoMapper;
            _colourMapper = colourMapper;
        }

        public GradeAccessory MapGradeAccessory(Administration.ModelGenerationGradeAccessory generationGradeAccessory, Administration.ModelGenerationAccessory generationAccessory, Administration.Accessory crossModelAccessory, Administration.EquipmentCategories categories, IReadOnlyList<Administration.Car> cars, Boolean isPreview)
        {
            var mappedGradeEquipmentItem = new GradeAccessory
            {
            };

            return MapGradeEquipmentItem(mappedGradeEquipmentItem, generationGradeAccessory, generationAccessory, crossModelAccessory, categories, cars, isPreview);
        }

        public GradeOption MapGradeOption(Administration.ModelGenerationGradeOption generationGradeOption, Administration.ModelGenerationOption generationOption, Administration.Option crossModelOption, Administration.EquipmentCategories categories, IReadOnlyList<Administration.Car> cars, Boolean isPreview)
        {
            if (generationGradeOption.HasParentOption && !generationGradeOption.ParentOption.ShortID.HasValue)
                throw new CorruptDataException(String.Format("Please supply a ShortID for grade option {0}", generationGradeOption.ParentOption.ID));

            var mappedGradeEquipmentItem = new GradeOption
            {
                TechnologyItem = generationOption.TechnologyItem,
                ParentOptionShortID = generationGradeOption.HasParentOption ? generationGradeOption.ParentOption.ShortID.Value : 0
            };

            return MapGradeEquipmentItem(mappedGradeEquipmentItem, generationGradeOption, generationOption, crossModelOption, categories, cars, isPreview);
        }

        public IDictionary<Guid, GradeEquipment> MapSubModelGradeEquipment(ModelGenerationSubModel modelGenerationSubModel, ContextData contextData)
        {
            var cars = modelGenerationSubModel.Cars();
            var allGrades = contextData.Grades;

            var grades = allGrades.Where(grade => cars.Any(car => car.GradeID == grade.ID));

            var applicableEquipment = grades.SelectMany(grade => contextData.GradeEquipment 
                                                    .Where(equipment => equipment.Key == grade.ID)).ToList();

            var mappedSubModelGradeEquipment = new Dictionary<Guid, GradeEquipment>();

            foreach (var kvp in applicableEquipment)
            {
                var mappedAccessories = kvp.Value.Accessories.Select(accessory => 
                    MapSubModelGradeAccessory(accessory,modelGenerationSubModel.Cars()
                                                            .Where(car => car.GradeID== kvp.Key),kvp.Key))
                    .Where(gradeAccessory => gradeAccessory.OptionalOn.Count != 0 || gradeAccessory.StandardOn.Count != 0)
                    .Select(accessory => accessory)
                    .ToList();

                var mappedOptions = kvp.Value.Options.Select(option => 
                    MapSubModelGradeOption(option, modelGenerationSubModel.Cars()
                                                        .Where(car => car.GradeID == kvp.Key), kvp.Key))
                    .Where(gradeAccessory => gradeAccessory.OptionalOn.Count != 0 || gradeAccessory.StandardOn.Count != 0)
                    .Select(accessory => accessory)
                    .ToList();

                mappedSubModelGradeEquipment.Add(kvp.Key,new GradeEquipment {Accessories = mappedAccessories,Options = mappedOptions});
            }


            return mappedSubModelGradeEquipment;
        }

        private GradeAccessory MapSubModelGradeAccessory(GradeAccessory accessory, IEnumerable<Car> gradeCars, Guid gradeID)
        {
            var mappedAccessory = new GradeAccessory();

            return MapSubModelGradeEquipmentItem(mappedAccessory,accessory, gradeID, gradeCars);
        }

        private GradeOption MapSubModelGradeOption(GradeOption option, IEnumerable<Car> gradeCars,Guid gradeID)
        {
            var mappedGradeOption = new GradeOption();

            return MapSubModelGradeEquipmentItem(mappedGradeOption, option, gradeID,
                gradeCars);
        }

        private IReadOnlyList<CarInfo> GetAvailabilityInfoForSubModelGradeEquipment(GradeEquipmentItem equipmentItem, Availability availability, IEnumerable<Car> cars, Guid gradeID)
        {
            return cars.Where(car => car.Equipment[equipmentItem.ID] != null ? car.Equipment[equipmentItem.ID].Availability == availability : availability == Availability.NotAvailable)
                        .Select(car => new CarInfo
                        {
                            Name = car.Translation.Name.DefaultIfEmpty(car.Name),
                            ShortID = car.ShortID.Value
                        })
                        .ToList();
        }

        T MapGradeEquipmentItem<T>(T mappedEquipmentItem, ModelGenerationGradeEquipmentItem generationGradeEquipmentItem, ModelGenerationEquipmentItem generationEquipmentItem, Administration.EquipmentItem crossModelEquipmentItem, Administration.EquipmentCategories categories, IReadOnlyList<Administration.Car> cars, Boolean isPreview)
            where T : GradeEquipmentItem
        {
            if (!generationGradeEquipmentItem.ShortID.HasValue)
                throw new CorruptDataException(String.Format("Please supply a ShortID for grade equipment item {0}", generationGradeEquipmentItem.ID));

            var hasColour = generationGradeEquipmentItem.Colour.ID != Guid.Empty;
            var isOwner = generationGradeEquipmentItem.Owner == MyContext.GetContext().CountryCode;

            mappedEquipmentItem.BestVisibleIn = null;
            mappedEquipmentItem.Category = _categoryInfoMapper.MapEquipmentCategoryInfo(generationGradeEquipmentItem.Category, categories); // ??
            mappedEquipmentItem.Description = generationGradeEquipmentItem.Translation.Description;
            mappedEquipmentItem.ExteriorColour = hasColour ? GetColour(generationEquipmentItem, isPreview) : null;
            mappedEquipmentItem.FootNote = generationGradeEquipmentItem.Translation.FootNote;
            mappedEquipmentItem.GradeFeature = generationGradeEquipmentItem.GradeFeature;
            mappedEquipmentItem.ID = generationGradeEquipmentItem.ID;
            mappedEquipmentItem.InternalName = generationEquipmentItem.BaseName;
            mappedEquipmentItem.KeyFeature = generationEquipmentItem.KeyFeature;
            mappedEquipmentItem.Labels = generationGradeEquipmentItem.Translation.Labels
                                                                                 .Select(_labelMapper.MapLabel)
                                                                                 .Where(label => !String.IsNullOrWhiteSpace(label.Value))
                                                                                 .ToList();
            mappedEquipmentItem.Links = crossModelEquipmentItem.Links.Select(link => _linkMapper.MapLink(link, isPreview)).ToList();
            mappedEquipmentItem.Name = generationGradeEquipmentItem.Translation.Name.DefaultIfEmpty(generationGradeEquipmentItem.Name);
            mappedEquipmentItem.NotAvailableOn = GetAvailabilityInfo(generationGradeEquipmentItem, Availability.NotAvailable, cars);
            mappedEquipmentItem.OptionalGradeFeature = generationGradeEquipmentItem.OptionalGradeFeature;
            mappedEquipmentItem.OptionalOn = GetAvailabilityInfo(generationGradeEquipmentItem, Availability.Optional, cars);
            mappedEquipmentItem.PartNumber = generationGradeEquipmentItem.PartNumber;
            mappedEquipmentItem.Path = Administration.MyContext.GetContext().EquipmentGroups.Find(generationGradeEquipmentItem.Group.ID).Path.ToLowerInvariant();
            mappedEquipmentItem.ShortID = generationGradeEquipmentItem.ShortID.Value;
            mappedEquipmentItem.SortIndex = generationGradeEquipmentItem.Index;
            mappedEquipmentItem.StandardOn = GetAvailabilityInfo(generationGradeEquipmentItem, Availability.Standard, cars);
            mappedEquipmentItem.Visibility = _visibilityMapper.MapVisibility(generationEquipmentItem.Visibility);
            mappedEquipmentItem.ToolTip = generationGradeEquipmentItem.Translation.ToolTip;
            mappedEquipmentItem.InternalCode = generationEquipmentItem.BaseCode;
            mappedEquipmentItem.LocalCode = generationGradeEquipmentItem.LocalCode.DefaultIfEmpty(isOwner ? generationEquipmentItem.BaseCode : String.Empty);

            mappedEquipmentItem.NotAvailable = mappedEquipmentItem.NotAvailableOn.Count > mappedEquipmentItem.StandardOn.Count &&
                                               mappedEquipmentItem.NotAvailableOn.Count > mappedEquipmentItem.OptionalOn.Count;

            mappedEquipmentItem.Optional = mappedEquipmentItem.OptionalOn.Count > mappedEquipmentItem.StandardOn.Count &&
                                           mappedEquipmentItem.OptionalOn.Count >= mappedEquipmentItem.NotAvailableOn.Count;

            mappedEquipmentItem.Standard = mappedEquipmentItem.StandardOn.Count >= mappedEquipmentItem.OptionalOn.Count &&
                                           mappedEquipmentItem.StandardOn.Count >= mappedEquipmentItem.NotAvailableOn.Count;
            
            return mappedEquipmentItem;
        }

        T MapSubModelGradeEquipmentItem<T>(T mappedEquipmentItem , GradeEquipmentItem equipmentItem, Guid gradeID, IEnumerable<Car> cars)
            where T : GradeEquipmentItem
        {
                mappedEquipmentItem.OptionalOn = GetAvailabilityInfoForSubModelGradeEquipment(equipmentItem,Availability.Optional, cars, gradeID);
                mappedEquipmentItem.NotAvailableOn = GetAvailabilityInfoForSubModelGradeEquipment(equipmentItem,Availability.NotAvailable, cars, gradeID);
                mappedEquipmentItem.StandardOn = GetAvailabilityInfoForSubModelGradeEquipment(equipmentItem, Availability.Standard, cars, gradeID);
                mappedEquipmentItem.BestVisibleIn = equipmentItem.BestVisibleIn;
                mappedEquipmentItem.Category = equipmentItem.Category;
                mappedEquipmentItem.Description = equipmentItem.Description;
                mappedEquipmentItem.ExteriorColour = equipmentItem.ExteriorColour;
                mappedEquipmentItem.FootNote = equipmentItem.FootNote;
                mappedEquipmentItem.GradeFeature = equipmentItem.GradeFeature;
                mappedEquipmentItem.ID = equipmentItem.ID;
                mappedEquipmentItem.InternalName = equipmentItem.InternalName;
                mappedEquipmentItem.KeyFeature = equipmentItem.KeyFeature;
                mappedEquipmentItem.Labels = equipmentItem.Labels;
                mappedEquipmentItem.Links = equipmentItem.Links;
                mappedEquipmentItem.Name = equipmentItem.Name;
                mappedEquipmentItem.OptionalGradeFeature = equipmentItem.OptionalGradeFeature;
                mappedEquipmentItem.PartNumber = equipmentItem.PartNumber;
                mappedEquipmentItem.Path = equipmentItem.Path;
                mappedEquipmentItem.ShortID = equipmentItem.ShortID;
                mappedEquipmentItem.SortIndex = equipmentItem.SortIndex;
                mappedEquipmentItem.Visibility = equipmentItem.Visibility;
                mappedEquipmentItem.ToolTip = equipmentItem.ToolTip;
                mappedEquipmentItem.InternalCode = equipmentItem.InternalCode;
                mappedEquipmentItem.LocalCode = equipmentItem.LocalCode;
                mappedEquipmentItem.NotAvailable = equipmentItem.NotAvailableOn.Count > equipmentItem.StandardOn.Count && equipmentItem.NotAvailableOn.Count > equipmentItem.OptionalOn.Count;
                mappedEquipmentItem.Optional = equipmentItem.OptionalOn.Count > equipmentItem.StandardOn.Count && equipmentItem.OptionalOn.Count >= equipmentItem.NotAvailableOn.Count;
            mappedEquipmentItem.Standard = equipmentItem.StandardOn.Count >= equipmentItem.OptionalOn.Count &&
                                           equipmentItem.StandardOn.Count >= equipmentItem.NotAvailableOn.Count;

            return mappedEquipmentItem;
        }

        ExteriorColour GetColour(ModelGenerationEquipmentItem generationEquipmentItem, Boolean isPreview)
        {
            var colour = generationEquipmentItem.Generation.ColourCombinations.ExteriorColours().FirstOrDefault(clr => clr.ID == generationEquipmentItem.Colour.ID);
            
            if (colour != null)
                return _colourMapper.MapExteriorColour(generationEquipmentItem.Generation, colour, isPreview);

            var crossModelColour = ExteriorColours.GetExteriorColours()[generationEquipmentItem.Colour.ID];
            return _colourMapper.MapExteriorColour(generationEquipmentItem.Generation, crossModelColour, isPreview);
        }

        IReadOnlyList<CarInfo> GetAvailabilityInfo(ModelGenerationGradeEquipmentItem generationGradeEquipmentItem, Availability availability, IEnumerable<Car> cars)
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
