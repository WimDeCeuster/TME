using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.QueryRepository.Service.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.QueryRepository.Service
{
    public class LanguageService : ILanguageService
    {
        private readonly ISerialiser _serialiser;
        private readonly IService _service;
        private readonly IKeyManager _keyManager;

        public LanguageService(ISerialiser serialiser, IService service, IKeyManager keyManager)
        {
            _serialiser = serialiser;
            _service = service;
            _keyManager = keyManager;
        }

        public Languages GetLanguages(IContext context)
        {
            var key = _keyManager.GetLanguagesKey();
            var serialisedLanguages = _service.GetObject(context.Brand, context.Country, key);
            return _serialiser.Deserialise<Languages>(serialisedLanguages);
        }
    }
}