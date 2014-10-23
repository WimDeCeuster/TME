using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Publisher.Interfaces;
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

        public EquipmentMapper(IBaseMapper baseMapper, ILabelMapper labelMapper, ILinkMapper linkMapper, IVisibilityMapper visibilityMapper, ICategoryInfoMapper categoryInfoMapper)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");
            if (labelMapper == null) throw new ArgumentNullException("labelMapper");
            if (linkMapper == null) throw new ArgumentNullException("linkMapper");
            if (visibilityMapper == null) throw new ArgumentNullException("visibilityMapper");
            if (categoryInfoMapper == null) throw new ArgumentNullException("categoryInfoMapper");

            _baseMapper = baseMapper;
            _labelMapper = labelMapper;
            _linkMapper = linkMapper;
            _visibilityMapper = visibilityMapper;
            _categoryInfoMapper = categoryInfoMapper;
        }

        public GradeAccessory MapGradeAccessory(Administration.ModelGenerationGradeAccessory generationGradeAccessory, Administration.ModelGenerationAccessory generationAccessory, Administration.Accessory crossModelAccessory, Boolean isPreview)
        {
            var mappedGradeEquipmentItem = new GradeAccessory
            {
                
            };

            return MapGradeEquipmentItem(mappedGradeEquipmentItem, generationGradeAccessory, generationAccessory, crossModelAccessory, isPreview);
        }

        public GradeOption MapGradeOption(Administration.ModelGenerationGradeOption generationGradeOption, Administration.ModelGenerationOption generationOption, Administration.Option crossModelOption, Boolean isPreview)
        {
            var mappedGradeEquipmentItem = new GradeOption
            {
                TechnologyItem = generationOption.TechnologyItem,
                //ParentOptionShortID = generationGradeOption.HasParentOption ? generationGradeOption.ParentOption.ShortID : 0 // ??
            };

            return MapGradeEquipmentItem(mappedGradeEquipmentItem, generationGradeOption, generationOption, crossModelOption, isPreview);
        }

        T MapGradeEquipmentItem<T>(T mappedEquipmentItem, Administration.ModelGenerationGradeEquipmentItem generationGradeEquipmentItem, Administration.ModelGenerationEquipmentItem generationEquipmentItem, Administration.EquipmentItem crossModelEquipmentItem, Boolean isPreview)
            where T : EquipmentItem
        {
            //var colour = Administration.ExteriorColours.GetExteriorColours()[generationEquipmentItem.Colour.ID];
            //
            //var x = generationEquipmentItem.Generation.ColourCombinations.ExteriorColours().First(colour => colour.ID == generationEquipmentItem.Colour.ID);
            //
            //// colour.Code; => xml item name
            //
            //var hostName = String.Empty; //ConfigurationManager.Apsettings... "hostname"?

            //generationEquipmentItem.Generation.Assets.First(asset => asset.AssetType.Code.StartsWith("colourschema", StringComparison.InvariantCultureIgnoreCase)).FileName;

            /*
             <colours method="rgb" model="AYGO">
                <item name="4W5">
                    <rgb>ff512d</rgb>
                    <brightness>-15</brightness>
                    <contrast>0</contrast>
                    <saturation>100</saturation>
                    <alpha>0.60</alpha>
                </item>
             </colours>
            */




            mappedEquipmentItem.Category = _categoryInfoMapper.MapEquipmentCategoryInfo(generationGradeEquipmentItem.Category); // ??
            mappedEquipmentItem.Description = generationGradeEquipmentItem.Translation.Description;
            mappedEquipmentItem.ExteriorColour = null; // ?? (transformation?)
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
    }
}
