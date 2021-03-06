﻿using System;
using System.Text;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.Shared
{
    public class KeyManager : IKeyManager
    {
        private const string PublicationKeyTemplate = "publication/{0}";
        private const string PublicationTimeFrameKeyTemplate = "publication/{0}/time-frame/{1}";
        private const string PublicationAssetsKeyTemplate = "publication/{0}/assets";

        public string GetModelsKey()
        {
            return "models-per-language.json";
        }

        private static string GetPublicationKey(Guid publicationID)
        {
            return string.Format(PublicationKeyTemplate, publicationID);
        }

        public string GetPublicationFileKey(Guid publicationID)
        {
            return string.Format("{0}.json", GetPublicationKey(publicationID));
        }

        private static string GetTimeFrameKey(Guid publicationID, Guid timeFrameID)
        {
            return string.Format(PublicationTimeFrameKeyTemplate, publicationID, timeFrameID);
        }

        public string GetBodyTypesKey(Guid publicationID, Guid timeFrameID)
        {
            return string.Format("{0}/body-types.json", GetTimeFrameKey(publicationID, timeFrameID));
        }

        public string GetEnginesKey(Guid publicationID, Guid timeFrameID)
        {
            return string.Format("{0}/engines.json", GetTimeFrameKey(publicationID, timeFrameID));
        }

        public string GetTransmissionsKey(Guid publicationID, Guid timeFrameID)
        {
            return string.Format("{0}/transmissions.json", GetTimeFrameKey(publicationID, timeFrameID));
        }

        public string GetWheelDrivesKey(Guid publicationID, Guid timeFrameID)
        {
            return string.Format("{0}/wheel-drives.json", GetTimeFrameKey(publicationID, timeFrameID));
        }

        public string GetSteeringsKey(Guid publicationID, Guid timeFrameID)
        {
            return string.Format("{0}/steerings.json", GetTimeFrameKey(publicationID, timeFrameID));
        }

        public string GetGradesKey(Guid publicationID, Guid timeFrameID)
        {
            return string.Format("{0}/grades.json", GetTimeFrameKey(publicationID, timeFrameID));
        }

        public string GetCarsKey(Guid publicationID, Guid timeFrameID)
        {
            return string.Format("{0}/cars.json", GetTimeFrameKey(publicationID, timeFrameID));
        }

        public string GetColourCombinationsKey(Guid publicationID, Guid timeFrameID)
        {
            return string.Format("{0}/colour-combinations.json", GetTimeFrameKey(publicationID, timeFrameID));
        }

        public string GetEquipmentCategoriesKey(Guid publicationID)
        {
            return string.Format("{0}/equipment-categories.json", GetPublicationKey(publicationID));
        }

        public string GetSpecificationCategoriesKey(Guid publicationID)
        {
            return string.Format("{0}/specification-categories.json", GetPublicationKey(publicationID));
        }

        public string GetSubModelsKey(Guid publicationID, Guid timeFrameID)
        {
            return string.Format("{0}/submodels.json", GetTimeFrameKey(publicationID, timeFrameID));
        }

        private string GetGradeKey(Guid publicationID, Guid timeFrameID, Guid gradeID)
        {
            return string.Format("{0}/grade/{1}", GetTimeFrameKey(publicationID, timeFrameID), gradeID);
        }

        public string GetSubModelGradeEquipmentsKey(Guid publicationID, Guid timeFrameID, Guid gradeID, Guid subModelID)
        {
            return string.Format("{0}/grade/{1}/equipment.json", GetSubModelKey(publicationID, timeFrameID, subModelID), gradeID);
        }

        public string GetSubModelGradePacksKey(Guid publicationID, Guid timeFrameID, Guid gradeID, Guid subModelID)
        {
            return string.Format("{0}/grade/{1}/packs.json", GetSubModelKey(publicationID, timeFrameID, subModelID), gradeID);
        }

        public string GetSubModelGradesKey(Guid publicationID, Guid timeFrameID, Guid subModelID)
        {
            return string.Format("{0}/grades.json", GetSubModelKey(publicationID, timeFrameID, subModelID));
        }

        private string GetSubModelKey(Guid publicationID, Guid timeFrameID, Guid subModelID)
        {
            return string.Format("{0}/submodel/{1}", GetTimeFrameKey(publicationID, timeFrameID), subModelID);
        }

        public string GetGradeEquipmentsKey(Guid publicationID, Guid timeFrameID, Guid gradeID)
        {
            return string.Format("{0}/equipment.json", GetGradeKey(publicationID, timeFrameID, gradeID));
        }

        public string GetGradePacksKey(Guid publicationID, Guid timeFrameID, Guid gradeID)
        {
            return string.Format("{0}/packs.json", GetGradeKey(publicationID, timeFrameID, gradeID));
        }

        private static string GetAssetsKeyPrefix(Guid publicationId, Guid objectId)
        {
            var publicationAssetsKey = string.Format(PublicationAssetsKeyTemplate, publicationId);

            return string.Format("{0}/{1}", publicationAssetsKey, objectId);
        }

        private static string GetDefaultAssetsKey(string assetsKey)
        {
            return string.Format("{0}/default.json", assetsKey);
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

        public string GetAssetsKey(Guid publicationId, Guid objectId, string view, string mode)
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

        public string GetCarPartsKey(Guid publicationID, Guid carID)
        {
            return String.Format("{0}/car/{1}/car-parts.json", GetPublicationKey(publicationID), carID);
        }
        
        public string GetCarEquipmentKey(Guid publicationID, Guid carID)
        {
            return String.Format("{0}/car/{1}/equipment.json", GetPublicationKey(publicationID), carID);
        }

        public string GetCarTechnicalSpecificationsKey(Guid publicationID, Guid carID)
        {
            return String.Format("{0}/car/{1}/specs.json", GetPublicationKey(publicationID), carID);
        }

        public string GetCarPacksKey(Guid publicationID, Guid carID)
        {
            return String.Format("{0}/car/{1}/packs.json", GetPublicationKey(publicationID), carID);
        }

        public string GetCarPartAssetsKey(Guid publicationID, Guid carID, string view, string mode)
        {
            return GetAssetsKeyWithViewAndMode(String.Format("{0}/car/{1}/assets/car-parts", GetPublicationKey(publicationID), carID), view, mode);
        }

        public string GetDefaultCarEquipmentAssetsKey(Guid publicationID, Guid carID)
        {
            return String.Format("{0}/car/{1}/assets/car-equipment/default.json", GetPublicationKey(publicationID), carID);
        }

        public string GetCarEquipmentAssetsKey(Guid publicationID, Guid carID, string view, string mode)
        {
            return GetAssetsKeyWithViewAndMode(String.Format("{0}/car/{1}/assets/car-equipment", GetPublicationKey(publicationID), carID), view, mode);
        }

        public string GetCarColourCombinationsKey(Guid publicationID, Guid carID)
        {
            return String.Format("{0}/car/{1}/colour-combinations.json", GetPublicationKey(publicationID), carID);
        }

        public string GetCarRulesKey(Guid publicationID, Guid carID)
        {
            return String.Format("{0}/car/{1}/rules.json", GetPublicationKey(publicationID), carID);
        }

        public string GetCarPackAccentColourCombinationsKey(Guid publicationID, Guid carID)
        {
            return String.Format("{0}/car/{1}/pack-accent-colour-combinations.json", GetPublicationKey(publicationID), carID);
        }

        private static string GetAssetsKeyWithViewAndMode(string assetsKey, string view, string mode)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(assetsKey);
            stringBuilder.Append("/");
            stringBuilder.Append(view);

            if (string.IsNullOrEmpty(mode))
            {
                stringBuilder.Append(".json");
                return stringBuilder.ToString();
            }

            stringBuilder.Append("/");
            stringBuilder.Append(mode);

            stringBuilder.Append(".json");

            return stringBuilder.ToString();
        }
    }
}
