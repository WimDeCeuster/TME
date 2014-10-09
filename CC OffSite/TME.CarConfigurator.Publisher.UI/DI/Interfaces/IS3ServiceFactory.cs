using System;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Enums;
using IPublicationService = TME.CarConfigurator.Publisher.Interfaces.IPublicationService;

namespace TME.CarConfigurator.Publisher.UI.DI.Interfaces
{
    public interface IS3ServiceFactory
    {
        ILanguageService GetLanguageService(String environment, PublicationDataSubset dataSubset);
        IPublicationService GetPublicationService(String environment, PublicationDataSubset dataSubset);
        IBodyTypeService GetBodyTypeService(String environment, PublicationDataSubset dataSubset);
        IEngineService GetEngineService(String environment, PublicationDataSubset dataSubset);
    }
}
