using System;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.Shared
{
    public class KeyManager : IKeyManager
    {
        private const string LanguagesKey = "models-per-language";
        private const string PublicationKeyTemplate = "publication/{0}";
        private const string PublicationTimeFrameKeyTemplate = "publication/{0}/time-frame/{1}";
        private const string PublicationAssetsKeyTemplate = "publication/{0}/assets";
        const String GenerationAssetsKeyTemplate = "publication/{0}/assets/default";

        public string GetLanguagesKey()
        {
            return LanguagesKey;
        }

        public string GetPublicationKey(Guid publicationID)
        {
            return string.Format(PublicationKeyTemplate, publicationID);
        }

        private static string GetTimeFrameKey(Guid publicationID, Guid timeFrameID)
        {
            return string.Format(PublicationTimeFrameKeyTemplate, publicationID, timeFrameID);
        }

        public string GetBodyTypesKey(Guid publicationID, Guid timeFrameID)
        {
            return string.Format("{0}/{1}", GetTimeFrameKey(publicationID, timeFrameID), "body-types");
        }

        public string GetGenerationAssetKey(Guid publicationID)
        {
            return String.Format(GenerationAssetsKeyTemplate, publicationID);
        }

        public string GetEnginesKey(Guid publicationID, Guid timeFrameID)
        {
            return string.Format("{0}/{1}", GetTimeFrameKey(publicationID, timeFrameID), "engines");
        }

        private static string GetAssetsKey(Guid publicationId, Guid objectId)
        {
            var publicationAssetsKey = string.Format(PublicationAssetsKeyTemplate, publicationId);
            
            return string.Format("{0}/{1}", publicationAssetsKey, objectId);
        }

        public string GetDefaultAssetsKey(Guid publicationId, Guid objectId)
        {
            return string.Format("{0}/default", GetAssetsKey(publicationId, objectId));
        }

        public string GetAssetsKey(Guid publicationId, Guid objectId, string view, string mode)
        {
            return string.Format("{0}/{1}/{2}", GetAssetsKey(publicationId, objectId), view, mode);
        }
    }
}
