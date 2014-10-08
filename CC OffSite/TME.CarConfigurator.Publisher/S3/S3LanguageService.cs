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
            if (service == null) throw new ArgumentNullException("service");
            if (serialiser == null) throw new ArgumentNullException("serialiser");

            _service = service;
            _serialiser = serialiser;
        }

        public Languages GetModelsOverviewPerLanguage()
        {
            try
            {
                var value = _service.GetObject(_modelsOverviewPath);
                return _serialiser.Deserialise<Languages>(value);
            }
            catch (ObjectNotFoundException)
            {
                return new Languages();
            }
        }

        public async Task<Result> PutModelsOverviewPerLanguage(Languages languages)
        {
            return await _service.PutObjectAsync(_modelsOverviewPath, _serialiser.Serialise(languages));
        }
    }
}
