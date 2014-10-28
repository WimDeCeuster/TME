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

        public ExteriorColour MapExteriorColour(Administration.ModelGenerationExteriorColour colour, string colourFilePath)
        {
            var mappedColour = new ExteriorColour
            {
                Transformation = GetColourTransformation(colourFilePath, colour.Code),
                InternalCode = colour.Code,
                LocalCode = String.Empty,
                SortIndex = 0
            };

            return _baseMapper.MapTranslateableDefaults(mappedColour, colour);
        }

        public ExteriorColour MapColourCombination(ModelGenerationExteriorColour exteriorColour)
        {
            var mappedColour = new ExteriorColour()
            {
                InternalCode = exteriorColour.Code,
                LocalCode = String.Empty,
                SortIndex = 0
            };

            return _baseMapper.MapTranslateableDefaults(mappedColour, exteriorColour);
        }

        public ExteriorColour MapExteriorColour(Administration.ExteriorColour colour, string colourFilePath)
        {
            var mappedColour = new ExteriorColour
            {
                Transformation = GetColourTransformation(colourFilePath, colour.Code),
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
