using System;
using System.Text;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.Shared
{
    public class KeyManager : IKeyManager
    {
        private const string PUBLICATION_KEY_TEMPLATE = "publication/{0}";
        private const string PUBLICATION_TIME_FRAME_KEY_TEMPLATE = "publication/{0}/time-frame/{1}";
        private const string PUBLICATION_ASSETS_KEY_TEMPLATE = "publication/{0}/assets";

        public string GetModelsKey()
        {
            return "models-per-language";
        }

        public string GetPublicationKey(Guid publicationID)
        {
            return string.Format(PUBLICATION_KEY_TEMPLATE, publicationID);
        }

        private static string GetTimeFrameKey(Guid publicationID, Guid timeFrameID)
        {
            return string.Format(PUBLICATION_TIME_FRAME_KEY_TEMPLATE, publicationID, timeFrameID);
        }

        public string GetBodyTypesKey(Guid publicationID, Guid timeFrameID)
        {
            return string.Format("{0}/body-types", GetTimeFrameKey(publicationID, timeFrameID));
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

        public string GetSteeringsKey(Guid publicationID, Guid timeFrameID)
        {
            return string.Format("{0}/steerings", GetTimeFrameKey(publicationID, timeFrameID));
        }

        public string GetGradesKey(Guid publicationID, Guid timeFrameID)
        {
            return string.Format("{0}/grades", GetTimeFrameKey(publicationID, timeFrameID));
        }

        public string GetCarsKey(Guid publicationID, Guid timeFrameID)
        {
            return string.Format("{0}/cars", GetTimeFrameKey(publicationID, timeFrameID));
        }

        public string GetColourCombinationsKey(Guid publicationID, Guid timeFrameID)
        {
            return string.Format("{0}/colour-combinations", GetTimeFrameKey(publicationID, timeFrameID));
        }

        public string GetEquipmentCategoriesKey(Guid publicationID, Guid timeFrameID)
        {
            return string.Format("{0}/equipment-categories", GetTimeFrameKey(publicationID, timeFrameID));
        }

        public string GetSpecificationCategoriesKey(Guid publicationID, Guid timeFrameID)
        {
            return string.Format("{0}/specification-categories", GetTimeFrameKey(publicationID, timeFrameID));
        }

        public string GetSubModelsKey(Guid publicationID, Guid timeFrameID)
        {
            return string.Format("{0}/submodels", GetTimeFrameKey(publicationID, timeFrameID));
        }

        private string GetGradeKey(Guid publicationID, Guid timeFrameID, Guid gradeID)
        {
            return string.Format("{0}/grade/{1}", GetTimeFrameKey(publicationID, timeFrameID), gradeID);
        }

        public string GetSubModelGradeEquipmentsKey(Guid publicationID, Guid timeFrameID, Guid gradeID, Guid subModelID)
        {
            return string.Format("{0}/grade/{1}/equipment", GetSubModelKey(publicationID, timeFrameID, subModelID), gradeID);
        }

        public string GetSubModelGradePacksKey(Guid publicationID, Guid timeFrameID, Guid gradeID, Guid subModelID)
        {
            return string.Format("{0}/grade/{1}/packs", GetSubModelKey(publicationID, timeFrameID, subModelID), gradeID);
        }

        public string GetSubModelGradesKey(Guid publicationID, Guid timeFrameID, Guid subModelID)
        {
            return string.Format("{0}/grades", GetSubModelKey(publicationID, timeFrameID, subModelID));
        }

        private string GetSubModelKey(Guid publicationID, Guid timeFrameID, Guid subModelID)
        {
            return string.Format("{0}/submodel/{1}", GetTimeFrameKey(publicationID, timeFrameID), subModelID);
        }

        public string GetGradeEquipmentsKey(Guid publicationID, Guid timeFrameID, Guid gradeID)
        {
            return string.Format("{0}/equipment", GetGradeKey(publicationID, timeFrameID, gradeID));
        }

        public string GetGradePacksKey(Guid publicationID, Guid timeFrameID, Guid gradeID)
        {
            return string.Format("{0}/packs", GetGradeKey(publicationID, timeFrameID, gradeID));
        }

        private static string GetAssetsKeyPrefix(Guid publicationId, Guid objectId)
        {
            var publicationAssetsKey = string.Format(PUBLICATION_ASSETS_KEY_TEMPLATE, publicationId);

            return string.Format("{0}/{1}", publicationAssetsKey, objectId);
        }

        private static string GetDefaultAssetsKey(string assetsKey)
        {
            return string.Format("{0}/default", assetsKey);
        }

        public string GetDefaultAssetsKey(Guid publicationId, Guid objectId)
        {
            var assetsKey = GetAssetsKeyPrefix(publicationId, objectId);

            return GetDefaultAssetsKey(assetsKey);
        }

        public string GetDefaultCarAssetsKey(Guid publicationID, Guid carID, Guid objectID)
        {
            var assetsKey = GetPublicationAssetsKey(publicationID, carID, "car", objectID);

            return GetDefaultAssetsKey(assetsKey);
        }

        public string GetDefaultSubModelAssetsKey(Guid publicationID, Guid subModelID, Guid objectID)
        {
            var assetsKey = GetPublicationAssetsKey(publicationID, subModelID, "submodel", objectID);

            return GetDefaultAssetsKey(assetsKey);
        }

        private string GetPublicationAssetsKey(Guid publicationID, Guid objectID, string objectName, Guid subObjectID)
        {
            return string.Format("{0}/{1}/{2}/assets/{3}", GetPublicationKey(publicationID), objectName, objectID, subObjectID);
        }

        public string GetAssetsKey(Guid publicationId, Guid objectId, String view, String mode)
        {
            return GetAssetsKeyWithViewAndMode(GetAssetsKeyPrefix(publicationId, objectId), view, mode);
        }

        public string GetCarAssetsKey(Guid publicationID, Guid carID, Guid objectID, string view, string mode)
        {
            return GetAssetsKeyWithViewAndMode(GetPublicationAssetsKey(publicationID, carID, "car", objectID), view, mode);
        }

        public string GetSubModelAssetsKey(Guid publicationID, Guid subModelID, Guid objectID, string view, string mode)
        {
            return GetAssetsKeyWithViewAndMode(GetPublicationAssetsKey(publicationID, subModelID, "submodel", objectID), view, mode);
        }

        public string GetCarCarPartsKey(Guid publicationID, Guid carID)
        {
            return String.Format("{0}/car/{1}/carparts", GetPublicationKey(publicationID), carID);
        }

        public string GetCarPacksKey(Guid publicationID, Guid carID)
        {
            return String.Format("{0}/car/{1}/packs", GetPublicationKey(publicationID), carID);
        }

        private static string GetAssetsKeyWithViewAndMode(string assetsKey, string view, string mode)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(assetsKey);
            stringBuilder.Append("/");
            stringBuilder.Append(view);

            if (string.IsNullOrEmpty(mode))
                return stringBuilder.ToString();

            stringBuilder.Append("/");
            stringBuilder.Append(mode);

            return stringBuilder.ToString();
        }
    }
}
