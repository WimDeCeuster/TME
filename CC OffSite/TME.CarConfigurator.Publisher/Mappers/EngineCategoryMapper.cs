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
        readonly ILabelMapper _labelMapper;

        public EngineCategoryMapper(IAssetMapper assetMapper, ILabelMapper labelMapper)
        {
            if (assetMapper == null) throw new ArgumentNullException("assetMapper");
            if (labelMapper == null) throw new ArgumentNullException("labelMapper");

            _assetMapper = assetMapper;
            _labelMapper = labelMapper;
        }

        public EngineCategory MapEngineCategory(Administration.EngineCategory engineCategory)
        {
            return new EngineCategory
            {
                Assets = engineCategory.Assets.Select(_assetMapper.MapLinkedAsset).ToList(),
                Description = engineCategory.Translation.Description,
                FootNote = engineCategory.Translation.FootNote,
                ID = engineCategory.ID,
                InternalCode = engineCategory.Code,
                Labels = engineCategory.Translation.Labels.Select(_labelMapper.MapLabel).ToList(), 
                LocalCode = String.Empty,
                Name = engineCategory.Translation.Name.DefaultIfEmpty(engineCategory.Name),
                SortIndex = engineCategory.Index,
                ToolTip = engineCategory.Translation.ToolTip
            };
        }
    }
}
