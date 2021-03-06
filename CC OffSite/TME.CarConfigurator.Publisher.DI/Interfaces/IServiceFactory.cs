﻿using System;
using TME.CarConfigurator.CommandServices;
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
        IEquipmentService GetEquipmentService(String environment, PublicationDataSubset dataSubset);
        ISpecificationsService GetSpecificationsService(String environment, PublicationDataSubset dataSubset);
        IColourService GetColourCombinationService(String environment, PublicationDataSubset dataSubset);
        ICarPartService GetCarPartService(String environment, PublicationDataSubset dataSubset);
        IPackService GetPackService(String environment, PublicationDataSubset dataSubset);
        IRuleService GetRuleService(String environment, PublicationDataSubset dataSubset);
    }
}
