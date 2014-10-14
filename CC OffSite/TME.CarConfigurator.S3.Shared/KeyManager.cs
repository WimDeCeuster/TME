using System;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.Shared
{
    public class KeyManager : IKeyManager
    {
        const String LanguagesKey = "models-per-language";
        const String PublicationKeyTemplate = "publication/{0}";
        const String GenerationBodyTypesKeyTemplate = "publication/{0}/time-frame/{1}/body-types";
        const String GenerationEnginesKeyTemplate = "publication/{0}/time-frame/{1}/engines";

        public string GetLanguagesKey()
        {
            return LanguagesKey;
        }

        public string GetPublicationKey(Guid publicationID)
        {
            return String.Format(PublicationKeyTemplate, publicationID);
        }

        public string GetGenerationBodyTypesKey(Guid publicationID, Guid timeFrameID)
        {
            return String.Format(GenerationBodyTypesKeyTemplate, publicationID, timeFrameID);
        }

        public string GetGenerationEnginesKey(Guid publicationIdID, Guid timeFrameID)
        {
            return String.Format(GenerationEnginesKeyTemplate, publicationIdID, timeFrameID);
        }

        public string GetAssetsKey(Guid publicationId, Guid timeFrameId, Guid objectId)
        {
            throw new NotImplementedException();
        }
    }
}
