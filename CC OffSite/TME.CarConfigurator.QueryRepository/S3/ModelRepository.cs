using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.QueryRepository.Interfaces;
using TME.CarConfigurator.QueryRepository.Service.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.QueryRepository.S3
{
    public class ModelRepository : IModelRepository
    {
        private readonly ILanguageService _languageService;

        public ModelRepository(ILanguageService languageService)
        {
            if (languageService == null) throw new ArgumentNullException("languageService");

            _languageService = languageService;
        }

        public IEnumerable<Model> Get(IContext context)
        {
            var s3Languages = _languageService.Get();

            var s3Language = s3Languages.Single(l => l.Code.Equals(context.Language, StringComparison.InvariantCultureIgnoreCase));

            var intermediateModels = s3Language.Models.SelectMany(m => m.Publications.Select(p => new Model(m)));

            var models = new Models(intermediateModels);

            return models;
        }
    }
}