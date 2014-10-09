using Spring.Context.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.UI.DI.Interfaces;

namespace TME.CarConfigurator.Publisher.UI.DI.Facades
{
    public class S3PublisherFacade : IPublisherFacade
    {
        IS3ServiceFactory _serviceFactory;

        public S3PublisherFacade(IS3ServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public IPublisher GetPublisher(String environment, PublicationDataSubset dataSubset)
        {
            return (IPublisher)ContextRegistry.GetContext().CreateObject("Publisher", typeof(IPublisher), new Object[] {
                _serviceFactory.GetPublicationService(environment, dataSubset),
                _serviceFactory.GetPutLanguageService(environment, dataSubset),
                _serviceFactory.GetGetLanguageService(environment, dataSubset),
                _serviceFactory.GetBodyTypeService(environment, dataSubset),
                _serviceFactory.GetEngineService(environment, dataSubset)
            });
        }
    }
}
