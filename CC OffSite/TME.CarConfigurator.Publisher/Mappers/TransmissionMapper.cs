using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class TransmissionMapper : ITransmissionMapper
    {
        ILabelMapper _labelMapper;
        ITransmissionTypeMapper _transmissionTypeMapper;
        IAssetSetMapper _assetSetMapper;

        public TransmissionMapper(ILabelMapper labelMapper, ITransmissionTypeMapper transmissionTypeMapper, IAssetSetMapper assetSetMapper)
        {
            if (labelMapper == null) throw new ArgumentNullException("labelMapper");
            if (transmissionTypeMapper == null) throw new ArgumentNullException("transmissionTypeMapper");
            if (assetSetMapper == null) throw new ArgumentNullException("assetSetMapper");


            _labelMapper = labelMapper;
            _transmissionTypeMapper = transmissionTypeMapper;
            _assetSetMapper = assetSetMapper;
        }

        public Transmission MapTransmission(Administration.ModelGenerationTransmission generationTransmission)
        {
            var crossModelTransmission = Administration.Transmissions.GetTransmissions()[generationTransmission.ID];
            var transmissionType = Administration.TransmissionTypes.GetTransmissionTypes()[generationTransmission.Type.ID];

            return new Transmission
            {
                Brochure = generationTransmission.Brochure,
                Description = generationTransmission.Translation.Description,
                FootNote = generationTransmission.Translation.FootNote,
                ID = generationTransmission.ID,
                InternalCode = crossModelTransmission.BaseCode,
                KeyFeature = generationTransmission.KeyFeature,
                Labels = generationTransmission.Translation.Labels.Select(_labelMapper.MapLabel).ToList(),
                LocalCode = crossModelTransmission.LocalCode.DefaultIfEmpty(crossModelTransmission.BaseCode),
                Name = generationTransmission.Translation.Name.DefaultIfEmpty(generationTransmission.Name),
                NumberOfGears = generationTransmission.NumberOfGears,
                SortIndex = generationTransmission.Index,
                ToolTip = generationTransmission.Translation.ToolTip,
                Type = _transmissionTypeMapper.MapTransmissionType(transmissionType),
                VisibleIn = _assetSetMapper.GetVisibility(generationTransmission.AssetSet).ToList()
            };
        }
    }
}
