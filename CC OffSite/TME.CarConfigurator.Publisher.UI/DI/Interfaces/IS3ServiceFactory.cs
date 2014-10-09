using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.S3.PutServices.Interfaces;

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
