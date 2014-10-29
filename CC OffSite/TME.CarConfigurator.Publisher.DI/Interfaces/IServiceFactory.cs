﻿using System;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Publisher.Common.Enums;

namespace TME.CarConfigurator.Publisher.DI.Interfaces
{
    public interface IServiceFactory
    {
        IModelService GetPutModelService(String environment, PublicationDataSubset dataSubset);
        QueryServices.IModelService GetGetModelService(String environment, PublicationDataSubset dataSubset);
        IPublicationService GetPublicationService(String environment, PublicationDataSubset dataSubset);
        IBodyTypeService GetBodyTypeService(String environment, PublicationDataSubset dataSubset);
        IEngineService GetEngineService(String environment, PublicationDataSubset dataSubset);
        ITransmissionService GetTransmissionService(String environment, PublicationDataSubset dataSubset);
        IWheelDriveService GetWheelDriveService(String environment, PublicationDataSubset dataSubset);
        ISteeringService GetSteeringService(String environment, PublicationDataSubset dataSubset);
        IGradeService GetGradeService(String environment, PublicationDataSubset dataSubset);
        ICarService GetCarService(String environment, PublicationDataSubset dataSubset);
        IAssetService GetAssetService(String environment, PublicationDataSubset dataSubset);
        ISubModelService GetSubModelService(String environment, PublicationDataSubset dataSubset);
        IGradeEquipmentService GetGradeEquipmentService(String environment, PublicationDataSubset dataSubset);
        IGradePackService GetGradePackService(string environment, PublicationDataSubset dataSubset);
        IColourCombinationService GetColourCombinationService(String environment, PublicationDataSubset dataSubset);
    }
}
