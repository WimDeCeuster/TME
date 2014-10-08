using TME.CarConfigurator.QueryRepository.Service.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.QueryRepository.Service
{
    public class LanguageService : ILanguageService
    {
        private readonly ISerialiser _serialiser;
        private readonly IS3Service _service;
        private readonly IKeyManager _keyManager;

        public LanguageService(ISerialiser serialiser, IS3Service service, IKeyManager keyManager)
        {
            _serialiser = serialiser;
            _service = service;
            _keyManager = keyManager;
        }

        public Languages GetLanguages()
        {
            var key = _keyManager.GetLanguagesKey();
            var serialisedLanguages = _service.GetObject(key);
            return _serialiser.Deserialise<Languages>(serialisedLanguages);
        }
    }

    public interface ISerialiser
    {
        T Deserialise<T>(string serialisedObject);
    }

    public interface IS3Service
    {
        string GetObject(string key);
    }

    public interface IKeyManager
    {
        string GetLanguagesKey();
    }

}