using System;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.DI.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.S3.Publisher;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.CarConfigurator.Publisher.DI.S3
{
    public class PublisherFactory : ITargetPublisherFactory
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly ITimeFramePublishHelper _timeFramePublishHelper;

        private PublisherFactory(IServiceFactory serviceFactory, ITimeFramePublishHelper timeFramePublishHelper)
        {
            if (serviceFactory == null) throw new ArgumentNullException("serviceFactory");
            if (timeFramePublishHelper == null) throw new ArgumentNullException("timeFramePublishHelper");

            _serviceFactory = serviceFactory;
            _timeFramePublishHelper = timeFramePublishHelper;
        }

        private IModelPublisher GetModelPublisher(IModelService service)
        {
            return new ModelPublisher(service);
        }

        private IPublicationPublisher GetPublicationPublisher(IPublicationService service)
        {
            return new PublicationPublisher(service);
        }

        private IBodyTypePublisher GetBodyTypePublisher(IBodyTypeService service)
        {
            return new BodyTypePublisher(service, _timeFramePublishHelper);
        }

        private IEnginePublisher GetEnginePublisher(IEngineService service)
        {
            return new EnginePublisher(service, _timeFramePublishHelper);
        }

        private ITransmissionPublisher GetTransmissionPublisher(ITransmissionService service)
        {
            return new TransmissionPublisher(service, _timeFramePublishHelper);
        }

        private IWheelDrivePublisher GetWheelDrivePublisher(IWheelDriveService service)
        {
            return new WheelDrivePublisher(service, _timeFramePublishHelper);
        }

        private ISteeringPublisher GetSteeringPublisher(ISteeringService service)
        {
            return new SteeringPublisher(service, _timeFramePublishHelper);
        }

        private IGradePublisher GetGradePublisher(IGradeService service)
        {
            return new GradePublisher(service, _timeFramePublishHelper);
        }

        private ICarPublisher GetCarPublisher(ICarService service)
        {
            return new CarPublisher(service, _timeFramePublishHelper);
        }

        private IAssetPublisher GetAssetPublisher(IAssetService service)
        {
            return new AssetPublisher(service, _timeFramePublishHelper);
        }

        private IColourPublisher GetColourCombinationPublisher(IColourService service)
        {
            return new ColourPublisher(service, _timeFramePublishHelper);
        }

        private ISubModelPublisher GetSubModelPublisher(ISubModelService service)
        {
            return new SubModelPublisher(service, _timeFramePublishHelper);
        }

        private IEquipmentPublisher GetEquipmentPublisher(IEquipmentService service)
        {
            return new EquipmentPublisher(service, _timeFramePublishHelper);
        }

        private ISpecificationsPublisher GetSpecificationsPublisher(ISpecificationsService service)
        {
            return new SpecificationsPublisher(service, _timeFramePublishHelper);
        }

        private IGradePackPublisher GetGradePackPublisher(IGradePackService service)
        {
            return new GradePackPublisher(service, _timeFramePublishHelper);
        }

        public IPublisher GetPublisher(string environment, PublicationDataSubset dataSubset)
        {
            return new Publisher(
                GetPublicationPublisher(_serviceFactory.GetPublicationService(environment, dataSubset)),
                GetModelPublisher(_serviceFactory.GetPutModelService(environment, dataSubset)),
                _serviceFactory.GetGetModelService(environment, dataSubset),
                GetBodyTypePublisher(_serviceFactory.GetBodyTypeService(environment, dataSubset)),
                GetEnginePublisher(_serviceFactory.GetEngineService(environment, dataSubset)),
                GetTransmissionPublisher(_serviceFactory.GetTransmissionService(environment, dataSubset)),
                GetWheelDrivePublisher(_serviceFactory.GetWheelDriveService(environment, dataSubset)),
                GetSteeringPublisher(_serviceFactory.GetSteeringService(environment, dataSubset)),
                GetGradePublisher(_serviceFactory.GetGradeService(environment, dataSubset)),
                GetCarPublisher(_serviceFactory.GetCarService(environment, dataSubset)),
                GetAssetPublisher(_serviceFactory.GetAssetService(environment, dataSubset)),
                GetSubModelPublisher(_serviceFactory.GetSubModelService(environment, dataSubset)),
                GetEquipmentPublisher(_serviceFactory.GetEquipmentService(environment, dataSubset)),
                GetSpecificationsPublisher(_serviceFactory.GetSpecificationsService(environment, dataSubset)),
                GetGradePackPublisher(_serviceFactory.GetGradePackService(environment, dataSubset)),
                GetColourCombinationPublisher(_serviceFactory.GetColourCombinationService(environment,dataSubset))
            );
        }
    }
}
