using System;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.PutServices.Interfaces;
using TME.CarConfigurator.S3.Shared;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.S3.Shared.Result;

namespace TME.CarConfigurator.S3.PutServices
{
    public class LanguageService : ILanguageService
    {
        readonly IService _service;
        readonly ISerialiser _serialiser;
        readonly IKeyManager _keyManager;

        public LanguageService(IService service, ISerialiser serialiser, IKeyManager keyManager)
        {
            _service = service;
            _serialiser = serialiser;
            _keyManager = keyManager;
        }

        public async Task<Result> PutModelsOverviewPerLanguage(IContext context, Languages languages)
        {
            if (context == null) throw new ArgumentNullException("context");

            return await _service.PutObjectAsync(context.Brand, context.Country, _keyManager.GetLanguagesKey(), _serialiser.Serialise(languages));
        }
    }
}
