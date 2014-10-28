using System;
using System.Collections.Generic;
using System.Linq;  
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class ModelMapper : IModelMapper
    {
        readonly IBaseMapper _baseMapper;

        public ModelMapper(IBaseMapper baseMapper)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");

            _baseMapper = baseMapper;
        }

        public Model MapModel(Administration.Model model)
        {
            var mappedModel = new Model
            {
                Brand = model.Brand.Name,
                Promoted = model.Promoted,
                Publications = new List<PublicationInfo>()
            };

            return _baseMapper.MapDefaultsWithSort(mappedModel, model, model);
        }
    }
}
