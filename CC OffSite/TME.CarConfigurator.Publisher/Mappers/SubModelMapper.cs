using System;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class SubModelMapper : ISubModelMapper
    {
        private readonly ILabelMapper _labelMapper;

        public SubModelMapper(ILabelMapper labelMapper)
        {
            if (labelMapper == null) throw new ArgumentNullException("labelMapper");

            _labelMapper = labelMapper;
        }

        public SubModel MapSubModel(ModelGenerationSubModel modelGenerationSubModel)
        {
            return new SubModel()
            {
                Description = modelGenerationSubModel.Translation.Description,
                FootNote = modelGenerationSubModel.Translation.FootNote,
                ID = modelGenerationSubModel.ID,
                InternalCode = modelGenerationSubModel.BaseCode,
                Labels = modelGenerationSubModel.Translation.Labels.Select(_labelMapper.MapLabel).ToList(),
                LocalCode = modelGenerationSubModel.LocalCode.DefaultIfEmpty(modelGenerationSubModel.BaseCode),
                Name = modelGenerationSubModel.Translation.Name.DefaultIfEmpty(modelGenerationSubModel.Name),
                SortIndex = modelGenerationSubModel.Index,
                ToolTip = modelGenerationSubModel.Translation.ToolTip
            };
        }
    }
}