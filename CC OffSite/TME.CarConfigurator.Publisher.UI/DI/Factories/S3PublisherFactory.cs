using Spring.Context.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.UI.DI.Interfaces;

namespace TME.CarConfigurator.Publisher.UI.DI.Factories
{
    public class S3PublisherFactory : IPublisherFactory
    {
        public IModelPublisher GetModelPublisher(IModelService service)
        {
            return (IModelPublisher)ContextRegistry.GetContext().GetObject("S3ModelPublisher", new Object[] { service });
        }

        public IPublicationPublisher GetPublicationPublisher(IPublicationService service)
        {
            return (IPublicationPublisher)ContextRegistry.GetContext().GetObject("S3PublicationPublisher", new Object[] { service });
        }

        public IBodyTypePublisher GetBodyTypePublisher(IBodyTypeService service)
        {
            return (IBodyTypePublisher)ContextRegistry.GetContext().GetObject("S3BodyTypePublisher", new Object[] { service });
        }

        public IEnginePublisher GetEnginePublisher(IEngineService service)
        {
            return (IEnginePublisher)ContextRegistry.GetContext().GetObject("S3EnginePublisher", new Object[] { service });
        }

        public ICarPublisher GetCarPublisher(ICarService service)
        {
            return (ICarPublisher)ContextRegistry.GetContext().GetObject("S3CarPublisher", new Object[] { service });
        }
    }
}
