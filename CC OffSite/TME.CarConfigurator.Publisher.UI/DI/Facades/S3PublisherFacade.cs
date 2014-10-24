using System;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.UI.DI.Interfaces;

namespace TME.CarConfigurator.Publisher.UI.DI.Facades
{
    public class S3PublisherFacade : IPublisherFacade
    {
        readonly IPublisherFactory _publisherFactory;
        readonly IServiceFactory _serviceFactory;

        public S3PublisherFacade(IPublisherFactory publisherFactory, IServiceFactory serviceFactory)
        {
            if (publisherFactory == null) throw new ArgumentNullException("publisherFactory");
            if (serviceFactory == null) throw new ArgumentNullException("serviceFactory");

            _publisherFactory = publisherFactory;
            _serviceFactory = serviceFactory;
        }

        public IPublisher GetPublisher(String environment, PublicationDataSubset dataSubset)
        {
            return new Publisher(
                _publisherFactory.GetPublicationPublisher(
                    _serviceFactory.GetPublicationService(environment, dataSubset)),
                _publisherFactory.GetModelPublisher(
                    _serviceFactory.GetPutModelService(environment, dataSubset)),
                _serviceFactory.GetGetModelService(environment, dataSubset),
                _publisherFactory.GetBodyTypePublisher(
                    _serviceFactory.GetBodyTypeService(environment, dataSubset)),
                _publisherFactory.GetEnginePublisher(
                    _serviceFactory.GetEngineService(environment, dataSubset)),
                _publisherFactory.GetTransmissionPublisher(
                    _serviceFactory.GetTransmissionService(environment, dataSubset)),
                _publisherFactory.GetWheelDrivePublisher(
                    _serviceFactory.GetWheelDriveService(environment, dataSubset)),
                _publisherFactory.GetSteeringPublisher(
                    _serviceFactory.GetSteeringService(environment, dataSubset)),
                _publisherFactory.GetGradePublisher(
                    _serviceFactory.GetGradeService(environment, dataSubset)),
                _publisherFactory.GetCarPublisher(
                    _serviceFactory.GetCarService(environment, dataSubset)),
                _publisherFactory.GetAssetPublisher(
                    _serviceFactory.GetAssetService(environment, dataSubset)),
                _publisherFactory.GetSubModelPublisher(
                    _serviceFactory.GetSubModelService(environment, dataSubset)),
                _publisherFactory.GetGradeAccessoryPublisher(
                    _serviceFactory.GetGradeAccessoryService(environment, dataSubset)),
                _publisherFactory.GetGradeOptionPublisher(
                    _serviceFactory.GetGradeOptionService(environment, dataSubset))
            );
        }
    }
}
