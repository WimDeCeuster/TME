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
        IBaseMapper _baseMapper;

        public GradeMapper(IBaseMapper baseMapper)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");

            _baseMapper = baseMapper;
        }

        public Grade MapGrade(Administration.ModelGenerationGrade generationGrade)
        {
            var mappedGrade = new Grade
            {
                FeatureText = null, //?
                LongDescription = null, //?
                Special = generationGrade.Special,
                StartingPrice = null, //?
            };

            return _baseMapper.MapDefaultsWithSort(mappedGrade, generationGrade, generationGrade, generationGrade.Name);
        }
    }
}
