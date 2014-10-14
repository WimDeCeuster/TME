using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class BodyTypeMapper : IBodyTypeMapper
    {
        readonly ILabelMapper _labelMapper;
        readonly IAssetSetMapper _assetSetMapper;

        public BodyTypeMapper(ILabelMapper labelMapper, IAssetSetMapper assetSetMapper)
        {
            if (labelMapper == null) throw new ArgumentNullException("labelMapper");
            if (assetSetMapper == null) throw new ArgumentNullException("assetSetMapper");

            _labelMapper = labelMapper;
            _assetSetMapper = assetSetMapper;
        }

        public BodyType MapBodyType(Administration.ModelGenerationBodyType generationBodyType)
        {
            var crossModelBodyType = Administration.BodyTypes.GetBodyTypes()[generationBodyType.ID];

            return new BodyType
            {
                Description = generationBodyType.Translation.Description,
                FootNote = generationBodyType.Translation.FootNote,
                ID = generationBodyType.ID,
                InternalCode = crossModelBodyType.BaseCode,
                Labels = generationBodyType.Translation.Labels.Select(_labelMapper.MapLabel).ToList(),
                LocalCode = crossModelBodyType.LocalCode.DefaultIfEmpty(crossModelBodyType.BaseCode),
                Name = generationBodyType.Translation.Name.DefaultIfEmpty(generationBodyType.Name),
                NumberOfDoors = generationBodyType.NumberOfDoors,
                NumberOfSeats = generationBodyType.NumberOfSeats,
                SortIndex = generationBodyType.Index,
                ToolTip = generationBodyType.Translation.ToolTip,
                VisibleIn = _assetSetMapper.GetVisibility(generationBodyType.AssetSet).ToList()
            };
        }
    }
}
