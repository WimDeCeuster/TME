using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.QueryServices.Exceptions;
using TME.CarConfigurator.S3.Shared.Exceptions;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.QueryServices
{
    public class ModelService : IModelService
    {
        private readonly ISerialiser _serialiser;
        private readonly IService _service;
        private readonly IKeyManager _keyManager;

        public ModelService(ISerialiser serialiser, IService service, IKeyManager keyManager)
        {
            _serialiser = serialiser;
            _service = service;
            _keyManager = keyManager;
        }

        public IEnumerable<Model> GetModels(Context context)
        {
            var languages = GetModelsByLanguage(context.Brand, context.Country);

            var language = languages.SingleOrDefault(l => l.Code.Equals(context.Language, StringComparison.InvariantCultureIgnoreCase));

            if (language != null) return language.Models;

            throw new CountryLanguageCombinationDoesNotExistException(context.Country, context.Language);
        }

        public Languages GetModelsByLanguage(string brand, string country)
        {
            var key = _keyManager.GetLanguagesKey();

            try
            {
                var serialisedLanguages = _service.GetObject(brand, country, key);
                return _serialiser.Deserialise<Languages>(serialisedLanguages);
            }
            catch (ObjectNotFoundException)
            {
                return new Languages();
            }
        }
    }
}