using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Colours;

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

        ColourTransformation GetColourTransformation(string colourFilePath, string colourCode)
        {
            var fileContent = _assetFileService.GetFileContent(colourFilePath);

            var xml = XDocument.Parse(fileContent);

            var item = xml.Root.Elements("item").FirstOrDefault(el => el.Attribute("name").Value.Equals(colourCode, StringComparison.InvariantCultureIgnoreCase));

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

            return new ColourTransformation
            {
                Alpha = Decimal.Parse(item.Element("alpha").Value, invariantCulture),
                Brightness = Decimal.Parse(item.Element("brightness").Value, invariantCulture),
                Contrast = Decimal.Parse(item.Element("contrast").Value, invariantCulture),
                Hue = 0,
                RGB = item.Element("rgb").Value,
                Saturation = Decimal.Parse(item.Element("saturation").Value, invariantCulture)
            };
        }
    }
}
