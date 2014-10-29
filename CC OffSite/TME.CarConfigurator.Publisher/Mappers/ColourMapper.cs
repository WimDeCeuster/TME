using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Colours;
using ExteriorColour = TME.CarConfigurator.Repository.Objects.Colours.ExteriorColour;
using Upholstery = TME.CarConfigurator.Repository.Objects.Colours.Upholstery;
using UpholsteryType = TME.CarConfigurator.Repository.Objects.Colours.UpholsteryType;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class ColourMapper : IColourMapper
    {
        IBaseMapper _baseMapper;
        IAssetFileService _assetFileService;


        public ColourMapper(IBaseMapper baseMapper, IAssetFileService assetFileService)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");
            if (assetFileService == null) throw new ArgumentNullException("assetFileService");

            _baseMapper = baseMapper;
            _assetFileService = assetFileService;
        }

        public ExteriorColour MapExteriorColour(ModelGeneration modelGeneration, Administration.ModelGenerationExteriorColour colour, Boolean isPreview)
        {
            var mappedColour = new ExteriorColour
            {
                Transformation = GetColourTransformation(modelGeneration, colour.Code, isPreview),
                InternalCode = colour.Code,
                LocalCode = String.Empty,
                SortIndex = 0
            };

            return _baseMapper.MapTranslateableDefaults(mappedColour, colour);
        }

        public ColourCombination MapColourCombination(ModelGeneration modelGeneration, ModelGenerationColourCombination colourCombination, Boolean isPreview)
        {
            return new ColourCombination
            {
                ExteriorColour = MapExteriorColour(modelGeneration, colourCombination.ExteriorColour, isPreview),
                ID = colourCombination.ID,
                SortIndex = 0, //?
                Upholstery = MapUpholstery(colourCombination.Upholstery)
            };
        }

        public ExteriorColour MapExteriorColour(ModelGeneration modelGeneration, Administration.ExteriorColour colour, Boolean isPreview)
        {
            var mappedColour = new ExteriorColour
            {
                Transformation = GetColourTransformation(modelGeneration, colour.Code, isPreview),
                InternalCode = colour.Code,
                LocalCode = String.Empty,
                SortIndex = 0
            };

            return _baseMapper.MapTranslateableDefaults(mappedColour, colour);
        }

        Upholstery MapUpholstery(ModelGenerationUpholstery modelGenerationUpholstery)
        {
            var mappedUpholstery = new Upholstery
            {
                InternalCode = modelGenerationUpholstery.Code,
                LocalCode = String.Empty,
                InteriorColourCode = modelGenerationUpholstery.InteriorColour.Code,
                TrimCode = modelGenerationUpholstery.Trim.Code,
                Type = MapUpholsteryType(modelGenerationUpholstery.Type)
            };

            return _baseMapper.MapSortDefaults(
                        _baseMapper.MapTranslateableDefaults(mappedUpholstery, modelGenerationUpholstery),
                        modelGenerationUpholstery);
        }

        UpholsteryType MapUpholsteryType(UpholsteryTypeInfo upholsteryTypeInfo)
        {
            var upholstery = Administration.UpholsteryTypes.GetUpholsteryTypes()[upholsteryTypeInfo.ID];
            
            var mappedUpholsteryType = new UpholsteryType
            {
                InternalCode = upholstery.Code,
                LocalCode = String.Empty,
                SortIndex = upholstery.Index
            };

            return _baseMapper.MapTranslateableDefaults(mappedUpholsteryType, upholstery);
        }

        ColourTransformation GetColourTransformation(ModelGeneration generation, String colourCode, Boolean isPreview)
        {
            var colourSchemaAsset = GetColourSchemaAsset(generation, isPreview);
            if (colourSchemaAsset == null)
                return new ColourTransformation();

            var fileContent = _assetFileService.GetFileContent(colourSchemaAsset.FileName);

            var xml = XDocument.Parse(fileContent);

            var item = xml.Root.Elements("item").FirstOrDefault(el => el.Attribute("name").Value.Equals(colourCode, StringComparison.InvariantCultureIgnoreCase));

            if (item == null)
                return new ColourTransformation();

            var invariantCulture = CultureInfo.GetCultureInfo(String.Empty);

            var alphaItem = item.Element("alpha");
            var brightnessItem = item.Element("brightness");
            var contrastItem = item.Element("contrast");
            var hueItem = item.Element("hue");
            var rgbItem = item.Element("rgb");
            var saturationItem = item.Element("saturation");

            var rgbValue = rgbItem == null ? String.Empty : rgbItem.Value;

            // sometimes the rgb values will have its leading 0 truncated "because Flash can handle it"
            // here we restore sanity
            if (rgbValue.Length == 5)
                rgbValue = "0" + rgbValue;

            return new ColourTransformation
            {
                Alpha = alphaItem == null ? 0 : Decimal.Parse(alphaItem.Value, invariantCulture),
                Brightness = brightnessItem == null ? 0 : Decimal.Parse(brightnessItem.Value, invariantCulture),
                Contrast = contrastItem == null ? 0 : Decimal.Parse(contrastItem.Value, invariantCulture),
                Hue = hueItem == null ? 0 : Decimal.Parse(hueItem.Value, invariantCulture),
                RGB = rgbItem == null ? String.Empty : rgbItem.Value,
                Saturation = saturationItem == null ? 0 : Decimal.Parse(saturationItem.Value, invariantCulture)
            };

        }

        static TME.CarConfigurator.Administration.Assets.LinkedAsset GetColourSchemaAsset(ModelGeneration generation, Boolean isPreview)
        {
            var liveTypeName = "colourschema";
            var previewTypeName = "preview_" + liveTypeName;
            var activeVersionName = generation.ActiveCarConfiguratorVersion.Name;
            var preferredTypeName = isPreview ? previewTypeName : liveTypeName;
            var preferredVersionedTypeName = String.Format("{0}-{1}", preferredTypeName, activeVersionName);
            var liveVersionedTypeName = String.Format("{0}-{1}", liveTypeName, activeVersionName);

            return generation.Assets[preferredVersionedTypeName] ?? // preview_colourschema-4.x
                   generation.Assets[liveVersionedTypeName] ?? // colourschema-4.x
                   generation.Assets[preferredTypeName] ?? // preview_colourschema
                   generation.Assets[liveTypeName]; //// colourschema
        }
    }
}