using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class SteeringMapper : ISteeringMapper
    {
        ILabelMapper _labelMapper;

        public SteeringMapper(ILabelMapper labelMapper)
        {
            if (labelMapper == null) throw new ArgumentNullException("labelMapper");

            _labelMapper = labelMapper;
        }

        public Steering MapSteering(Administration.Steering steering)
        {
            return new Steering
            {
                Description = steering.Translation.Description,
                FootNote = steering.Translation.FootNote,
                ID = steering.ID,
                InternalCode = steering.BaseCode,
                Labels = steering.Translation.Labels.Select(_labelMapper.MapLabel).ToList(),
                LocalCode = steering.LocalCode.DefaultIfEmpty(steering.BaseCode),
                Name = steering.Translation.Name.DefaultIfEmpty(steering.Name),
                SortIndex = 0,
                ToolTip = steering.Translation.ToolTip
            };
        }
    }
}
