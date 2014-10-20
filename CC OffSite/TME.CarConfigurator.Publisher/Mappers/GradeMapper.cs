using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class GradeMapper : IGradeMapper
    {
        ILabelMapper _labelMapper;

        public GradeMapper(ILabelMapper labelMapper)
        {
            if (labelMapper == null) throw new ArgumentNullException("labelMapper");

            _labelMapper = labelMapper;
        }

        public Grade MapGrade(Administration.ModelGenerationGrade generationGrade)
        {
            return new Grade
            {
                Description = generationGrade.Translation.Description,
                FeatureText = null, //?
                FootNote = generationGrade.Translation.FootNote,
                ID = generationGrade.ID,
                InternalCode = generationGrade.BaseCode,
                Labels = generationGrade.Translation.Labels.Select(_labelMapper.MapLabel).ToList(),
                LocalCode = generationGrade.LocalCode.DefaultIfEmpty(generationGrade.BaseCode),
                LongDescription = null, //?
                Name = generationGrade.Translation.Name.DefaultIfEmpty(generationGrade.Name),
                SortIndex = generationGrade.Index,
                Special = generationGrade.Special,
                StartingPrice = null, //?
                ToolTip = generationGrade.Translation.ToolTip
            };
        }
    }
}
