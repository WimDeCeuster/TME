using TME.CarConfigurator.QueryRepository.Service;
using TME.CarConfigurator.QueryRepository.Service.Interfaces;
using TME.CarConfigurator.QueryRepository.Tests.GivenALanguageService;

namespace TME.CarConfigurator.QueryRepository.Tests.TestBuilders
{
    public class LanguageServiceBuilder
    {
        private ISerialiser _serialiser;
        private IS3Service _service;
        private IKeyManager _keyManager;

        public static LanguageServiceBuilder Initialize()
        {
            return new LanguageServiceBuilder();
        }

        public ILanguageService Build()
        {
            return new LanguageService(_serialiser, _service, _keyManager);
        }

        public LanguageServiceBuilder WithSerializer(ISerialiser serialiser)
        {
            _serialiser = serialiser;

            return this;
        }

        public LanguageServiceBuilder WithService(IS3Service service)
        {
            _service = service;

            return this;
        }

        public LanguageServiceBuilder WithKeyManager(IKeyManager keyManager)
        {
            _keyManager = keyManager;

            return this;
        }
    }
}