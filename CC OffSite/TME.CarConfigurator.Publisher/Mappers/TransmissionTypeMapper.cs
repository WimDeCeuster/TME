using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class TransmissionTypeMapper : ITransmissionTypeMapper
    {
        ILabelMapper _labelMapper;

        public TransmissionTypeMapper(ILabelMapper labelMapper)
        {
            if (labelMapper == null) throw new ArgumentNullException("labelMapper");

            _labelMapper = labelMapper;
        }

        public TransmissionType MapTransmissionType(Administration.TransmissionType transmissionType)
        {
            return new TransmissionType
            {
                Description = transmissionType.Translation.Description,
                FootNote = transmissionType.Translation.FootNote,
                ID = transmissionType.ID,
                InternalCode = transmissionType.BaseCode,
                Labels = transmissionType.Translation.Labels.Select(_labelMapper.MapLabel).ToList(),
                LocalCode = transmissionType.LocalCode.DefaultIfEmpty(transmissionType.BaseCode),
                Name = transmissionType.Translation.Name.DefaultIfEmpty(transmissionType.Name),
                SortIndex = 0,
                ToolTip = transmissionType.Translation.ToolTip
            };
        }
    }
}
