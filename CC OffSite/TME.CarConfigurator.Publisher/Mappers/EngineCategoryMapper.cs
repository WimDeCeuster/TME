using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class EngineCategoryMapper : IEngineCategoryMapper
    {
        readonly IAssetMapper _assetMapper;
        readonly IBaseMapper _baseMapper;

        public EngineCategoryMapper(IAssetMapper assetMapper, IBaseMapper baseMapper)
        {
            if (assetMapper == null) throw new ArgumentNullException("assetMapper");
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");

            _assetMapper = assetMapper;
            _baseMapper = baseMapper;
        }

        public EngineCategory MapEngineCategory(Administration.EngineCategory engineCategory)
        {
            var mappedEngineCategory = new EngineCategory
            {
                Assets = engineCategory.Assets.Select(_assetMapper.MapLinkedAsset).ToList(),
                InternalCode = engineCategory.Code,
                LocalCode = String.Empty
            };

            return _baseMapper.MapTranslateableDefaults(
                _baseMapper.MapSortDefaults(mappedEngineCategory, engineCategory),
                engineCategory,
                engineCategory.Name);
        }
    }
}
