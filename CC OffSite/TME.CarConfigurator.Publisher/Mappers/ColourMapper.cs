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

        public ExteriorColour MapExteriorColour(ModelGeneration modelGeneration, Administration.ModelGenerationExteriorColour colour)
        {
            var mappedColour = new ExteriorColour
            {
                Transformation = GetColourTransformation(GetFilePath(modelGeneration), colour.Code),
                InternalCode = colour.Code,
                LocalCode = String.Empty,
                SortIndex = 0
            };

            return _baseMapper.MapTranslateableDefaults(mappedColour, colour);
        }

        string GetFilePath(ModelGeneration modelGeneration)
        {
            return modelGeneration.Assets.First(asset => asset.AssetType.Name.StartsWith("colourschema", StringComparison.InvariantCultureIgnoreCase)).FileName;
        }

        public ColourCombination MapColourCombination(ModelGeneration modelGeneration, ModelGenerationColourCombination colourCombination)
        {
            return new ColourCombination
            {
                ExteriorColour = MapExteriorColour(modelGeneration, colourCombination.ExteriorColour),
                ID = colourCombination.ID,
                SortIndex = 0, //?
                Upholstery = MapUpholstery(colourCombination.Upholstery)
            };
        }

        Upholstery MapUpholstery(ModelGenerationUpholstery modelGenerationUpholstery)
        {
            
            var mappedUpholstery = new Upholstery
            {
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
                SortIndex = upholstery.Index
            };

            return _baseMapper.MapTranslateableDefaults(mappedUpholsteryType, upholstery);
        }

        public ExteriorColour MapExteriorColour(ModelGeneration modelGeneration, Administration.ExteriorColour colour)
        {
            var mappedColour = new ExteriorColour
            {
                Transformation = GetColourTransformation(GetFilePath(modelGeneration), colour.Code),
                InternalCode = colour.Code,
                LocalCode = String.Empty,
                SortIndex = 0
            };

            return _baseMapper.MapTranslateableDefaults(mappedColour, colour);
        }

        ColourTransformation GetColourTransformation(string colourFilePath, string colourCode)
        {
            var fileContent = _assetFileService.GetFileContent(colourFilePath);

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
    }
}
