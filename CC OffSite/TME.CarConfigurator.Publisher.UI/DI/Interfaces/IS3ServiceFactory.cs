using System;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Enums;

namespace TME.CarConfigurator.Publisher.UI.DI.Interfaces
{
    public interface IS3ServiceFactory
    {
        IModelService GetPutModelService(String environment, PublicationDataSubset dataSubset);
        QueryServices.IModelService GetGetModelService(String environment, PublicationDataSubset dataSubset);
        IPublicationService GetPublicationService(String environment, PublicationDataSubset dataSubset);
        IBodyTypeService GetBodyTypeService(String environment, PublicationDataSubset dataSubset);
        IEngineService GetEngineService(String environment, PublicationDataSubset dataSubset);
    }
}
