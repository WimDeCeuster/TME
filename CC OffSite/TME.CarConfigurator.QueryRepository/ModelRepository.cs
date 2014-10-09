using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.QueryRepository.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.QueryServices.Interfaces;

namespace TME.CarConfigurator.QueryRepository
{
    public class ModelRepository : IModelRepository
    {
        private readonly IModelService _modelService;

        public ModelRepository(IModelService modelService)
        {
            if (modelService == null) throw new ArgumentNullException("modelService");

            _modelService = modelService;
        }

        public IEnumerable<Model> GetModels(Context context)
        {
            var s3Languages = _modelService.GetModelsByLanguage(context.Brand, context.Country);

            var s3Language = s3Languages.Single(l => l.Code.Equals(context.Language, StringComparison.InvariantCultureIgnoreCase));

            return s3Language.Models;
        }
    }
}