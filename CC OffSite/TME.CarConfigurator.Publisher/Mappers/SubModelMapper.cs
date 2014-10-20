using System;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class SubModelMapper : ISubModelMapper
    {
        private readonly IBaseMapper _baseMapper;

        public SubModelMapper(IBaseMapper baseMapper)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");

            _baseMapper = baseMapper;
        }

        public SubModel MapSubModel(ModelGenerationSubModel modelGenerationSubModel)
        {
            var mappedSubModel = new SubModel();

            return _baseMapper.MapDefaultsWithSort(mappedSubModel, modelGenerationSubModel, modelGenerationSubModel, modelGenerationSubModel.Name);
        }
    }
}