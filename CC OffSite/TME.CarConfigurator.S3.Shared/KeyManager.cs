﻿using System;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.Shared
{
    public class KeyManager : IKeyManager
    {
        private const string PublicationKeyTemplate = "publication/{0}";
        private const string PublicationTimeFrameKeyTemplate = "publication/{0}/time-frame/{1}";
        private const string PublicationAssetsKeyTemplate = "publication/{0}/assets";
        const String GenerationAssetsKeyTemplate = "publication/{0}/assets/default";

        public string GetModelsKey()
        {
            return "models-per-language";
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
            return string.Format("{0}/body-types", GetTimeFrameKey(publicationID, timeFrameID));
        }

        public string GetGenerationAssetKey(Guid publicationID)
        {
            return String.Format(GenerationAssetsKeyTemplate, publicationID);
        }

        public string GetEnginesKey(Guid publicationID, Guid timeFrameID)
        {
            return string.Format("{0}/engines", GetTimeFrameKey(publicationID, timeFrameID));
        }

        public string GetTransmissionsKey(Guid publicationID, Guid timeFrameID)
        {
            return string.Format("{0}/transmissions", GetTimeFrameKey(publicationID, timeFrameID));
        }

        public string GetWheelDrivesKey(Guid publicationID, Guid timeFrameID)
        {
            return string.Format("{0}/wheel-drives", GetTimeFrameKey(publicationID, timeFrameID));
        }

        public string GetCarsKey(Guid publicationID, Guid timeFrameID)
        {
            return string.Format("{0}/cars", GetTimeFrameKey(publicationID, timeFrameID));
        }

        public string GetCarKey(Guid publicationID, Guid carID)
        {
            return string.Format("{0}/cars/{1}", GetPublicationKey(publicationID), carID);
        }

        private static string GetAssetsKey(Guid publicationId, Guid objectId)
        {
            var publicationAssetsKey = string.Format(PublicationAssetsKeyTemplate, publicationId);

            return string.Format("{0}/{1}", publicationAssetsKey, objectId);
        }

        private string GetAssetsKey(Guid publicationId, Guid carId, Guid objectId)
        {
            return string.Format("{0}/assets/{1}", GetCarKey(publicationId, carId), objectId);
        }

        public string GetDefaultAssetsKey(Guid publicationId, Guid objectId)
        {
            var assetsKey = GetAssetsKey(publicationId, objectId);

            return GetDefaultAssetsKey(assetsKey);
        }

        public string GetDefaultAssetsKey(Guid publicationID, Guid carID, Guid objectID)
        {
            var assetsKey = GetAssetsKey(publicationID, carID, objectID);

            return GetDefaultAssetsKey(assetsKey);
        }

        private static string GetDefaultAssetsKey(string assetsKey)
        {
            return string.Format("{0}/default", assetsKey);
        }

        public string GetAssetsKey(Guid publicationId, Guid objectId, String view, String mode)
        {
            return string.Format("{0}/{1}/{2}", GetAssetsKey(publicationId, objectId), view, mode);
        }

        public string GetAssetsKey(Guid publicationID, Guid carID, Guid objectID, string view, string mode)
        {
            return string.Format("{0}/{1}/{2}", GetAssetsKey(publicationID, carID, objectID), view, mode);
        }
    }
}
