using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Publisher.Interfaces;
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

        public GradeAccessory MapGradeAccessory(Administration.ModelGenerationGradeAccessory generationGradeAccessory, Administration.ModelGenerationAccessory generationAccessory, Administration.Accessory crossModelAccessory, Administration.EquipmentCategories categories, Boolean isPreview)
        {
            var mappedGradeEquipmentItem = new GradeAccessory
            {
                
            };

            return MapGradeEquipmentItem(mappedGradeEquipmentItem, generationGradeAccessory, generationAccessory, crossModelAccessory, categories, isPreview);
        }

        public GradeOption MapGradeOption(Administration.ModelGenerationGradeOption generationGradeOption, Administration.ModelGenerationOption generationOption, Administration.Option crossModelOption, Administration.EquipmentCategories categories, Boolean isPreview)
        {
            var mappedGradeEquipmentItem = new GradeOption
            {
                TechnologyItem = generationOption.TechnologyItem,
                //ParentOptionShortID = generationGradeOption.HasParentOption ? generationGradeOption.ParentOption.ShortID : 0 // ??
            };

            return MapGradeEquipmentItem(mappedGradeEquipmentItem, generationGradeOption, generationOption, crossModelOption, categories, isPreview);
        }

        T MapGradeEquipmentItem<T>(T mappedEquipmentItem, Administration.ModelGenerationGradeEquipmentItem generationGradeEquipmentItem, Administration.ModelGenerationEquipmentItem generationEquipmentItem, Administration.EquipmentItem crossModelEquipmentItem, Administration.EquipmentCategories categories, Boolean isPreview)
            where T : EquipmentItem
        {            
            var hasColour = generationGradeEquipmentItem.Colour.ID != Guid.Empty;

            mappedEquipmentItem.Category = _categoryInfoMapper.MapEquipmentCategoryInfo(generationGradeEquipmentItem.Category, categories); // ??
            mappedEquipmentItem.Description = generationGradeEquipmentItem.Translation.Description;
            mappedEquipmentItem.ExteriorColour = hasColour ? GetColour(generationEquipmentItem) : null;
            mappedEquipmentItem.FootNote = generationGradeEquipmentItem.Translation.FootNote;
            mappedEquipmentItem.GradeFeature = generationGradeEquipmentItem.GradeFeature;
            mappedEquipmentItem.ID = generationGradeEquipmentItem.ID;
            mappedEquipmentItem.InternalName = generationEquipmentItem.BaseName;
            mappedEquipmentItem.KeyFeature = generationEquipmentItem.KeyFeature; // ??
            mappedEquipmentItem.Labels = generationGradeEquipmentItem.Translation.Labels.Select(_labelMapper.MapLabel)
                                                                                   .Where(label => !String.IsNullOrWhiteSpace(label.Value))
                                                                                   .ToList();
            mappedEquipmentItem.Links = crossModelEquipmentItem.Links.Select(link => _linkMapper.MapLink(link, isPreview)).ToList();
            mappedEquipmentItem.Name = generationGradeEquipmentItem.Translation.Name.DefaultIfEmpty(generationGradeEquipmentItem.Name);
            mappedEquipmentItem.OptionalGradeFeature = generationGradeEquipmentItem.OptionalGradeFeature;
            mappedEquipmentItem.PartNumber = generationGradeEquipmentItem.PartNumber;
            mappedEquipmentItem.Path = Administration.MyContext.GetContext().EquipmentGroups.Find(generationGradeEquipmentItem.Group.ID).Path;
            mappedEquipmentItem.ShortID = 0; // ??
            mappedEquipmentItem.SortIndex = generationGradeEquipmentItem.Index;
            mappedEquipmentItem.Visibility = _visibilityMapper.MapVisibility(generationEquipmentItem.Visibility); // ??
            mappedEquipmentItem.ToolTip = generationGradeEquipmentItem.Translation.ToolTip;
            return _baseMapper.MapLocalizableDefaults(mappedEquipmentItem, generationEquipmentItem);
        }

        ExteriorColour GetColour(Administration.ModelGenerationEquipmentItem generationEquipmentItem)
        {
            var colour = generationEquipmentItem.Generation.ColourCombinations.ExteriorColours().FirstOrDefault(c => c.ID == generationEquipmentItem.Colour.ID);

            if (colour == null)
                return null;

            var colourFilePath = generationEquipmentItem.Generation.Assets.First(asset => asset.AssetType.Name.StartsWith("colourschema", StringComparison.InvariantCultureIgnoreCase)).FileName;

            
            return _colourMapper.MapExteriorColour(colour, colourFilePath);
        }
    }
}
