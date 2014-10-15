using Spring.Context.Support;
using System;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.UI.DI.Interfaces;

namespace TME.CarConfigurator.Publisher.UI.DI.Facades
{
    public class S3PublisherFacade : IPublisherFacade
    {
        IPublisherFactory _publisherFactory;
        IServiceFactory _serviceFactory;

        public S3PublisherFacade(IPublisherFactory publisherFactory, IServiceFactory serviceFactory)
        {
            _publisherFactory = publisherFactory;
            _serviceFactory = serviceFactory;
        }

        public IPublisher GetPublisher(String environment, PublicationDataSubset dataSubset)
        {
            return (IPublisher)ContextRegistry.GetContext().CreateObject("Publisher", typeof(IPublisher), new Object[] {
                _publisherFactory.GetPublicationPublisher(
                    _serviceFactory.GetPublicationService(environment, dataSubset)),
                _publisherFactory.GetModelPublisher(
                    _serviceFactory.GetPutModelService(environment, dataSubset)),
                _serviceFactory.GetGetModelService(environment, dataSubset),
                _publisherFactory.GetBodyTypePublisher(
                    _serviceFactory.GetBodyTypeService(environment, dataSubset)),
                _publisherFactory.GetEnginePublisher(
                    _serviceFactory.GetEngineService(environment, dataSubset)),
                _publisherFactory.GetCarPublisher(
                    _serviceFactory.GetCarService(environment, dataSubset)),
                _publisherFactory.GetAssetPublisher(
                    _serviceFactory.GetAssetService(environment,dataSubset))
            });
        }
    }
}
