using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.QueryRepository.Service.Base;
using TME.CarConfigurator.QueryRepository.Service.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.QueryRepository.Service
{
    public class LanguageService : ServiceBase, ILanguageService
    {

        public LanguageService(ISerialiser serialiser, IService service, IKeyManager keyManager)
            : base(serialiser, service, keyManager)
        {
        }

        public Languages GetLanguages(IContext context)
        {
            var key = KeyManager.GetLanguagesKey();
            var serialisedLanguages = Service.GetObject(context.Brand, context.Country, key);
            return Serialiser.Deserialise<Languages>(serialisedLanguages);
        }
    }

    public interface IKeyManager
    {
        string GetLanguagesKey();
    }

}