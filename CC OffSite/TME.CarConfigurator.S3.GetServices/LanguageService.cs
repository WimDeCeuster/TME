using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.GetServices.Interfaces;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.GetServices
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

        public Languages GetLanguages(Context context)
        {
            var key = _keyManager.GetLanguagesKey();
            var serialisedLanguages = _service.GetObject(context.Brand, context.Country, key);
            return _serialiser.Deserialise<Languages>(serialisedLanguages);
        }
    }
}