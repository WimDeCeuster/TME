using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Exceptions;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class EquipmentMapper : IEquipmentMapper
    {
        IBaseMapper _baseMapper;
        ILabelMapper _labelMapper;
        ILinkMapper _linkMapper;
        IVisibilityMapper _visibilityMapper;
        ICategoryInfoMapper _categoryInfoMapper;
        IColourMapper _colourMapper;

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

        private GradeAccessory MapAccessory(GradeAccessory gradeAccessory,IReadOnlyList<Car> applicableCars,Guid subModelID)
        {
            gradeAccessory.OptionalOn =
                applicableCars.Where(car => car.SubModel.ID == subModelID).Select(car => new CarInfo()
                {
                    Name = car.Name,
                    ShortID = car.ShortID
                }).ToList();
            return gradeAccessory;
        }

        T MapGradeEquipmentItem<T>(T mappedEquipmentItem, Administration.ModelGenerationGradeEquipmentItem generationGradeEquipmentItem, Administration.ModelGenerationEquipmentItem generationEquipmentItem, Administration.EquipmentItem crossModelEquipmentItem, Administration.EquipmentCategories categories, IReadOnlyList<Administration.Car> cars, Boolean isPreview)
            where T : GradeEquipmentItem
        {
            if (!generationGradeEquipmentItem.ShortID.HasValue)
                throw new CorruptDataException(String.Format("Please supply a ShortID for grade equipment item {0}", generationGradeEquipmentItem.ID));

            var hasColour = generationGradeEquipmentItem.Colour.ID != Guid.Empty;
            var isOwner = generationGradeEquipmentItem.Owner == Administration.MyContext.GetContext().CountryCode;

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
            mappedEquipmentItem.NotAvailableOn = GetAvailabilityInfo(generationGradeEquipmentItem, Administration.Enums.Availability.NotAvailable, cars);
            mappedEquipmentItem.OptionalGradeFeature = generationGradeEquipmentItem.OptionalGradeFeature;
            mappedEquipmentItem.OptionalOn = GetAvailabilityInfo(generationGradeEquipmentItem, Administration.Enums.Availability.Optional, cars);
            mappedEquipmentItem.PartNumber = generationGradeEquipmentItem.PartNumber;
            mappedEquipmentItem.Path = Administration.MyContext.GetContext().EquipmentGroups.Find(generationGradeEquipmentItem.Group.ID).Path.ToLowerInvariant();
            mappedEquipmentItem.ShortID = generationGradeEquipmentItem.ShortID.Value;
            mappedEquipmentItem.SortIndex = generationGradeEquipmentItem.Index;
            mappedEquipmentItem.StandardOn = GetAvailabilityInfo(generationGradeEquipmentItem, Administration.Enums.Availability.Standard, cars);
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

        ExteriorColour GetColour(Administration.ModelGenerationEquipmentItem generationEquipmentItem, Boolean isPreview)
        {
            var colour = generationEquipmentItem.Generation.ColourCombinations.ExteriorColours().FirstOrDefault(clr => clr.ID == generationEquipmentItem.Colour.ID);
            
            if (colour != null)
                return _colourMapper.MapExteriorColour(generationEquipmentItem.Generation, colour, isPreview);

            var crossModelColour = Administration.ExteriorColours.GetExteriorColours()[generationEquipmentItem.Colour.ID];
            return _colourMapper.MapExteriorColour(generationEquipmentItem.Generation, crossModelColour, isPreview);
        }

        IReadOnlyList<CarInfo> GetAvailabilityInfo(Administration.ModelGenerationGradeEquipmentItem generationGradeEquipmentItem, Administration.Enums.Availability availability, IReadOnlyList<Administration.Car> cars)
        {
            return cars.Where(car => car.Equipment[generationGradeEquipmentItem.ID] != null ? car.Equipment[generationGradeEquipmentItem.ID].Availability == availability : availability == Administration.Enums.Availability.NotAvailable)
                       .Select(car => new CarInfo
                       {
                           Name = car.Translation.Name.DefaultIfEmpty(car.Name),
                           ShortID = car.ShortID.Value
                       })
                       .ToList();
        }
    }
}
