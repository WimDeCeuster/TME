using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Enums.Result;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.S3.Exceptions;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.S3
{
    public class S3LanguageService : IS3LanguageService
    {
        readonly IS3Service _service;
        readonly IS3Serialiser _serialiser;
        readonly String _modelsOverviewPath = "models-per-language";

        public S3LanguageService(IS3Service service, IS3Serialiser serialiser)
        {
            _service = service ?? new S3Service(null);
            _serialiser = serialiser ?? new S3Serialiser();
        }

        public Languages GetModelsOverviewPerLanguage(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            try
            {
                var value = _service.GetObject(context.Brand, context.Country, _modelsOverviewPath);
                return _serialiser.Deserialise<Languages>(value);
            }
            catch (ObjectNotFoundException)
            {
                return new Languages();
            }
        }

        public async Task<Result> PutModelsOverviewPerLanguage(IContext context, Languages languages)
        {
            if (context == null) throw new ArgumentNullException("context");

            return await _service.PutObjectAsync(context.Brand, context.Country, _modelsOverviewPath, _serialiser.Serialise(languages));
        }
    }
}
