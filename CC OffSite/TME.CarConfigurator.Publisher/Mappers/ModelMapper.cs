using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class ModelMapper : IModelMapper
    {
        readonly ILabelMapper _labelMapper;

        public ModelMapper(ILabelMapper labelMapper)
        {
            if (labelMapper == null) throw new ArgumentNullException("labelMapper");

            _labelMapper = labelMapper;
        }

        public Model MapModel(Administration.Model model)
        {
            return new Model
            {
                Brand = model.Brand.Name,
                Description = model.Translation.Description,
                FootNote = model.Translation.FootNote,
                ID = model.ID,
                InternalCode = model.BaseCode,
                Labels = model.Translation.Labels.Select(_labelMapper.MapLabel).ToList(),
                LocalCode = model.LocalCode.DefaultIfEmpty(model.BaseCode),
                Name = model.Translation.Name.DefaultIfEmpty(model.Name),
                Promoted = model.Promoted,
                Publications = new List<PublicationInfo>(),
                SortIndex = model.Index,
                ToolTip = model.Translation.ToolTip
            };
        }
    }
}
