using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Exceptions;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.Repository.Objects.Core;
using ExteriorColour = TME.CarConfigurator.Repository.Objects.Colours.ExteriorColour;
using ExteriorColourInfo = TME.CarConfigurator.Repository.Objects.Colours.ExteriorColourInfo;
using ExteriorColourType = TME.CarConfigurator.Repository.Objects.Colours.ExteriorColourType;
using Upholstery = TME.CarConfigurator.Repository.Objects.Colours.Upholstery;
using UpholsteryInfo = TME.CarConfigurator.Repository.Objects.Colours.UpholsteryInfo;
using UpholsteryType = TME.CarConfigurator.Repository.Objects.Colours.UpholsteryType;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class ColourMapper : IColourMapper
    {
        readonly IBaseMapper _baseMapper;
        readonly IAssetSetMapper _assetSetMapper;
        readonly IAssetFileService _assetFileService;


        public ColourMapper(IBaseMapper baseMapper, IAssetSetMapper assetSetMapper, IAssetFileService assetFileService)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");
            if (assetSetMapper == null) throw new ArgumentNullException("assetSetMapper");
            if (assetFileService == null) throw new ArgumentNullException("assetFileService");

            _baseMapper = baseMapper;
            _assetSetMapper = assetSetMapper;
            _assetFileService = assetFileService;
        }

        public CarColourCombination MapLinkedColourCombination(ModelGeneration modelGeneration, LinkedColourCombination carColourCombination, Administration.ExteriorColourType exteriorColourType, Administration.UpholsteryType upholsteryType, bool isPreview, string assetUrl)
        {
            return new CarColourCombination
            {
                ExteriorColour = MapLinkedExteriorColour(modelGeneration, carColourCombination.ExteriorColour, exteriorColourType, isPreview, assetUrl),
                ID = carColourCombination.ID,
                SortIndex = 0,
                Upholstery = MapLinkedUpholstery(carColourCombination.Upholstery, upholsteryType),
                VisibleIn = new List<VisibleInModeAndView>()
            };
        }

        private CarExteriorColour MapLinkedExteriorColour(ModelGeneration modelGeneration, LinkedExteriorColour exteriorColour, Administration.ExteriorColourType exteriorColourType, bool isPreview, string assetUrl)
        {
            var mappedExteriorColour = new CarExteriorColour
            {
                Price = new Price
                {
                    ExcludingVat = exteriorColour.Price,
                    IncludingVat = exteriorColour.VatPrice
                },
                InternalCode = exteriorColour.Code,
                LocalCode = String.Empty,
                Promoted = exteriorColour.Promoted,
                SortIndex = exteriorColour.Index,
                VisibleIn = new List<VisibleInModeAndView>(),
                Transformation = GetColourTransformation(modelGeneration, exteriorColour.Code, isPreview, assetUrl),
                Type = MapExteriorColourType(exteriorColourType)
            };

            return _baseMapper.MapTranslateableDefaults(mappedExteriorColour, exteriorColour.GenerationExteriorColour);
        }

        public ColourCombination MapColourCombination(ModelGeneration modelGeneration, ModelGenerationColourCombination colourCombination, Boolean isPreview, Administration.ExteriorColourType exteriorColourType, Administration.UpholsteryType upholsteryType, String assetUrl)
        {
            return new ColourCombination
            {
                ExteriorColour = MapExteriorColour(modelGeneration, colourCombination.ExteriorColour, isPreview, exteriorColourType, assetUrl),
                ID = colourCombination.ID,
                SortIndex = 0, // will be replaced with list position based on exttype/ext/upholtype/uphol order in calling function
                Upholstery = MapUpholstery(colourCombination.Upholstery, upholsteryType)
            };
        }

        public ExteriorColour MapExteriorColour(ModelGeneration modelGeneration, ModelGenerationExteriorColour colour, Boolean isPreview, Administration.ExteriorColourType exteriorColourType, String assetUrl)
        {
            var mappedColour = new ExteriorColour
            {
                InternalCode = colour.Code,
                LocalCode = String.Empty,
                Promoted = colour.Promoted,
                SortIndex = colour.Index,
                Transformation = GetColourTransformation(modelGeneration, colour.Code, isPreview, assetUrl),
                Type = MapExteriorColourType(exteriorColourType),
                VisibleIn = _assetSetMapper.GetVisibility(colour.AssetSet, false).ToList()
            };

            return _baseMapper.MapTranslateableDefaults(mappedColour, colour);
        }

        public ExteriorColour MapExteriorColour(ModelGeneration modelGeneration, Administration.ExteriorColour colour, Boolean isPreview, Administration.ExteriorColourType exteriorColourType, String assetUrl)
        {
            var mappedColour = new ExteriorColour
            {
                InternalCode = colour.Code,
                LocalCode = String.Empty,
                Promoted = false,
                SortIndex = 0,
                Transformation = GetColourTransformation(modelGeneration, colour.Code, isPreview, assetUrl),
                Type = MapExteriorColourType(exteriorColourType),
                VisibleIn = _assetSetMapper.GetVisibility(colour.Assets, false).ToList()
            };

            _baseMapper.MapTranslateableDefaults(mappedColour, colour);

            //mappedColour.Name = colour.Translation.Name; //String.Empty;

            return mappedColour;
        }

        public ExteriorColourInfo MapExteriorColourInfo(CarPackExteriorColour exteriorColour)
        {
            return new ExteriorColourInfo
            {
                ID = exteriorColour.ID,
                InternalCode = exteriorColour.Code
            };
        }

        public ExteriorColourInfo MapExteriorColourInfo(Administration.ExteriorColourInfo colour)
        {
            return new ExteriorColourInfo
            {
                ID = colour.ID,
                InternalCode = colour.Code
            };
        }

        public ExteriorColourInfo MapExteriorColourApplicability(ExteriorColourApplicability applicability)
        {
            return new ExteriorColourInfo
                {
                    ID = applicability.ID,
                    InternalCode = applicability.Code
                };
        }

        private ExteriorColourType MapExteriorColourType(Administration.ExteriorColourType type)
        {
            var mappedType = new ExteriorColourType
            {
                InternalCode = type.Code,
                LocalCode = String.Empty,
                SortIndex = type.Index
            };

            return _baseMapper.MapTranslateableDefaults(mappedType, type);
        }

        private Upholstery MapUpholstery(ModelGenerationUpholstery modelGenerationUpholstery, Administration.UpholsteryType upholsteryType)
        {
            var mappedUpholstery = new Upholstery
            {
                InternalCode = modelGenerationUpholstery.Code,
                LocalCode = String.Empty,
                InteriorColourCode = modelGenerationUpholstery.InteriorColour.Code,
                TrimCode = modelGenerationUpholstery.Trim.Code,
                Type = MapUpholsteryType(upholsteryType),
                VisibleIn = _assetSetMapper.GetVisibility(modelGenerationUpholstery.AssetSet, false).ToList()
            };

            return _baseMapper.MapTranslateableDefaultsWithSort(mappedUpholstery, modelGenerationUpholstery);
        }

        private CarUpholstery MapLinkedUpholstery(LinkedUpholstery upholstery, Administration.UpholsteryType upholsteryType)
        {
            var mappedUpholstery = new CarUpholstery
            {
                Price = new Price
                { 
                    ExcludingVat = upholstery.Price, 
                    IncludingVat = upholstery.VatPrice
                },
                InteriorColourCode = upholstery.InteriorColour.Code,
                InternalCode = upholstery.Code,
                LocalCode = String.Empty,
                TrimCode = upholstery.Trim.Code,
                SortIndex = 0,
                Type = MapUpholsteryType(upholsteryType),
                VisibleIn = new List<VisibleInModeAndView>()
            };
            return _baseMapper.MapTranslateableDefaults(mappedUpholstery, upholstery.GenerationUpholstery);
        }

        private UpholsteryType MapUpholsteryType(Administration.UpholsteryType type)
        {
            var mappedUpholsteryType = new UpholsteryType
            {
                InternalCode = type.Code,
                LocalCode = String.Empty,
                SortIndex = type.Index
            };

            return _baseMapper.MapTranslateableDefaults(mappedUpholsteryType, type);
        }

        private ColourTransformation GetColourTransformation(ModelGeneration generation, String colourCode, Boolean isPreview, String assetUrl)
        {
            var colourSchemaAsset = GetColourSchemaAsset(generation, isPreview);
            if (colourSchemaAsset == null)
                return null;

            var fileContent = _assetFileService.GetFileContent(colourSchemaAsset.FileName, assetUrl);

            var root = XDocument.Parse(fileContent).Root;
            
            if (root == null)
                throw new CorruptDataException(string.Format("Colour transformation file {0} does not haqve a root element", colourSchemaAsset.FileName));
            
            var item = root.Descendants("item").FirstOrDefault(el => el.Attribute("name").Value.Equals(colourCode, StringComparison.InvariantCultureIgnoreCase));

            if (item == null)
                return null;

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
                RGB = rgbValue,
                Saturation = saturationItem == null ? 0 : Decimal.Parse(saturationItem.Value, invariantCulture)
            };
        }

        private static Administration.Assets.LinkedAsset GetColourSchemaAsset(ModelGeneration generation, Boolean isPreview)
        {
            const string colourschema = "colourschema";
            const string previewColourschema = "preview_colourschema";

            return generation.Assets[String.Format("{0}-{1}", isPreview ? previewColourschema : colourschema, generation.ActiveCarConfiguratorVersion.Name)] ?? // preview_colourschema-4.x
                   generation.Assets[String.Format("{0}-{1}", colourschema, generation.ActiveCarConfiguratorVersion.Name)] ?? // colourschema-4.x
                   generation.Assets[isPreview ? previewColourschema : colourschema] ?? // preview_colourschema
                   generation.Assets[colourschema]; //// colourschema
        }

        public UpholsteryInfo MapUpholsteryApplicability(UpholsteryApplicability applicability)
        {
            return new 
            UpholsteryInfo
                {
                    ID = applicability.ID,
                    InternalCode = applicability.Code
                };
        }

        public UpholsteryInfo MapUpholsteryInfo(Administration.UpholsteryInfo upholstery)
        {
            return new UpholsteryInfo
            {
                ID = upholstery.ID,
                InternalCode = upholstery.Code
            };
        }

        public UpholsteryInfo MapUpholsteryInfo(CarPackUpholstery upholstery)
        {
            return new UpholsteryInfo
            {
                ID = upholstery.ID,
                InternalCode = upholstery.Code
            };
        }
    }
}