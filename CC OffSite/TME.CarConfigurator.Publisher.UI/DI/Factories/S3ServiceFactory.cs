using Spring.Context.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.UI.DI.Interfaces;
using TME.CarConfigurator.S3.PutServices.Interfaces;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.Publisher.UI.DI.Factories
{
    public class S3ServiceFactory : IS3ServiceFactory
    {
        ISerialiser _serialiser;
        IKeyManager _keyManager;

        public S3ServiceFactory(ISerialiser serialiser, IKeyManager keyManager)
        {
            _serialiser = serialiser;
            _keyManager = keyManager;
        }

        IService GetService(String environment, PublicationDataSubset dataSubset)
        {
            var objectName = String.Format("{0}{1}S3Service", environment, dataSubset.ToString());
            return (IService)ContextRegistry.GetContext().GetObject(String.Format("{0}{1}S3Service", environment, dataSubset.ToString()));
        }

        public S3.GetServices.Interfaces.ILanguageService GetGetLanguageService(String environment, PublicationDataSubset dataSubset)
        {
            var service = GetService(environment, dataSubset);

            return (S3.GetServices.Interfaces.ILanguageService)ContextRegistry.GetContext().GetObject("S3GetLanguageService", new Object[] { _serialiser, service, _keyManager });
        }

        public ILanguageService GetPutLanguageService(String environment, PublicationDataSubset dataSubset)
        {
            var service = GetService(environment, dataSubset);

            return (ILanguageService)ContextRegistry.GetContext().GetObject("S3PutLanguageService", new Object[] { service, _serialiser, _keyManager });
        }

        public IPublicationService GetPublicationService(String environment, PublicationDataSubset dataSubset)
        {
            var service = GetService(environment, dataSubset);

            return (IPublicationService)ContextRegistry.GetContext().GetObject("S3PublicationService", new Object[] { service, _serialiser, _keyManager });
        }

        public IBodyTypeService GetBodyTypeService(String environment, PublicationDataSubset dataSubset)
        {
            var service = GetService(environment, dataSubset);

            return (IBodyTypeService)ContextRegistry.GetContext().GetObject("S3BodyTypeService", new Object[] { service, _serialiser, _keyManager });
        }

        public IEngineService GetEngineService(String environment, PublicationDataSubset dataSubset)
        {
            var service = GetService(environment, dataSubset);

            return (IEngineService)ContextRegistry.GetContext().GetObject("S3EngineService", new Object[] { service, _serialiser, _keyManager });
        }
    }
}
