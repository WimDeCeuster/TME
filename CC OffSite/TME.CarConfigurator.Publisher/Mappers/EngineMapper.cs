using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class EngineMapper : IEngineMapper
    {
        readonly ILabelMapper _labelMapper;
        readonly IEngineCategoryMapper _engineCategoryMapper;
        readonly IEngineTypeMapper _engineTypeMapper;
        readonly IAssetSetMapper _assetSetMapper;

        public EngineMapper(ILabelMapper labelMapper, IEngineCategoryMapper engineCategoryMapper, IEngineTypeMapper engineTypeMapper, IAssetSetMapper assetSetMapper)
        {
            if (labelMapper == null) throw new ArgumentNullException("labelMapper");
            if (engineCategoryMapper == null) throw new ArgumentNullException("engineCategoryMapper");
            if (engineTypeMapper == null) throw new ArgumentNullException("engineTypeMapper");
            if (assetSetMapper == null) throw new ArgumentNullException("assetSetMapper");

            _labelMapper = labelMapper;
            _engineCategoryMapper = engineCategoryMapper;
            _engineTypeMapper = engineTypeMapper;
            _assetSetMapper = assetSetMapper;
        }

        public Engine MapEngine(Administration.ModelGenerationEngine generationEngine)
        {
            var crossModelEngine = Administration.Engines.GetEngines()[generationEngine.ID];
            var engineCategory = Administration.EngineCategories.GetEngineCategories()[crossModelEngine.Category.ID];
            //var engineType = Administration.EngineTypes.GetEngineTypes()[engine.Type.ID];
            var fuelType = Administration.FuelTypes.GetFuelTypes()[generationEngine.Type.FuelType.ID];

            return new Engine
            {
                Brochure = generationEngine.Brochure,
                Category = engineCategory == null ? null : _engineCategoryMapper.MapEngineCategory(engineCategory),
                Description = generationEngine.Translation.Description,
                FootNote = generationEngine.Translation.FootNote,
                ID = generationEngine.ID,
                InternalCode = crossModelEngine.BaseCode,
                KeyFeature = generationEngine.KeyFeature,
                Labels = generationEngine.Translation.Labels.Select(_labelMapper.MapLabel).ToList(),
                LocalCode = crossModelEngine.LocalCode.DefaultIfEmpty(crossModelEngine.BaseCode),
                Name = generationEngine.Translation.Name.DefaultIfEmpty(generationEngine.Name),
                SortIndex = generationEngine.Index,
                ToolTip = generationEngine.Translation.ToolTip,
                Type = _engineTypeMapper.MapEngineType(generationEngine.Type),
                VisibleIn = _assetSetMapper.GetVisibility(generationEngine.AssetSet).ToList()
            };
        }
    }
}
