using System;
using System.Linq;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class EngineMapper : IEngineMapper
    {
        readonly IBaseMapper _baseMapper;
        readonly IEngineCategoryMapper _engineCategoryMapper;
        readonly IEngineTypeMapper _engineTypeMapper;
        readonly IAssetSetMapper _assetSetMapper;

        public EngineMapper(IBaseMapper baseMapper, IEngineCategoryMapper engineCategoryMapper, IEngineTypeMapper engineTypeMapper, IAssetSetMapper assetSetMapper)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");
            if (engineCategoryMapper == null) throw new ArgumentNullException("engineCategoryMapper");
            if (engineTypeMapper == null) throw new ArgumentNullException("engineTypeMapper");
            if (assetSetMapper == null) throw new ArgumentNullException("assetSetMapper");

            _baseMapper = baseMapper;
            _engineCategoryMapper = engineCategoryMapper;
            _engineTypeMapper = engineTypeMapper;
            _assetSetMapper = assetSetMapper;
        }

        public Engine MapEngine(Administration.ModelGenerationEngine generationEngine)
        {
            var crossModelEngine = Administration.Engines.GetEngines()[generationEngine.ID];
            var engineCategory = Administration.EngineCategories.GetEngineCategories()[crossModelEngine.Category.ID];

            var mappedEngine = new Engine
            {
                Brochure = generationEngine.Brochure,
                Category = engineCategory == null ? null : _engineCategoryMapper.MapEngineCategory(engineCategory),
                KeyFeature = generationEngine.KeyFeature,
                Type = _engineTypeMapper.MapEngineType(generationEngine.Type),
                VisibleIn = _assetSetMapper.GetVisibility(generationEngine.AssetSet).ToList()
            };

            return _baseMapper.MapDefaultsWithSort(mappedEngine, crossModelEngine, generationEngine);
        }
    }
}
