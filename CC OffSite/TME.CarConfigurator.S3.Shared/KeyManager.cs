using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.Shared
{
    public class KeyManager : IKeyManager
    {
        const String _languagesKey = "models-per-language";
        const String _publicationKeyTemplate = "publication/{0}";
        const String _generationBodyTypesKeyTemplate = "publication/{0}/time-frame/{1}/body-types";
        const String _generationEnginesKeyTemplate = "publication/{0}/time-frame/{1}/engines";

        public string GetLanguagesKey()
        {
            return _languagesKey;
        }

        public string GetPublicationKey(Publication publication)
        {
            return String.Format(_publicationKeyTemplate, publication.ID);
        }

        public string GetGenerationBodyTypesKey(Publication publication, PublicationTimeFrame timeFrame)
        {
            return String.Format(_generationBodyTypesKeyTemplate, publication.ID, timeFrame.ID);
        }

        public string GetGenerationEnginesKey(Publication publication, PublicationTimeFrame timeFrame)
        {
            return String.Format(_generationEnginesKeyTemplate, publication.ID, timeFrame.ID);
        }
    }
}
