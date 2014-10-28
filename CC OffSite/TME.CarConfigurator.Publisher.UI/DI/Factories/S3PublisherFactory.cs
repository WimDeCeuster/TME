using System;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.UI.DI.Interfaces;
using TME.CarConfigurator.S3.Publisher;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.CarConfigurator.Publisher.UI.DI.Factories
{
    public class S3PublisherFactory : IPublisherFactory
    {
        readonly ITimeFramePublishHelper _timeFramePublishHelper;

        public S3PublisherFactory(ITimeFramePublishHelper timeFramePublishHelper)
        {
            if (timeFramePublishHelper == null) throw new ArgumentNullException("timeFramePublishHelper");

            _timeFramePublishHelper = timeFramePublishHelper;
        }

        public IModelPublisher GetModelPublisher(IModelService service)
        {
            return new ModelPublisher(service);
        }

        public IPublicationPublisher GetPublicationPublisher(IPublicationService service)
        {
            return new PublicationPublisher(service);
        }

        public IBodyTypePublisher GetBodyTypePublisher(IBodyTypeService service)
        {
            return new BodyTypePublisher(service, _timeFramePublishHelper);
        }

        public IEnginePublisher GetEnginePublisher(IEngineService service)
        {
            return new EnginePublisher(service, _timeFramePublishHelper);
        }

        public ITransmissionPublisher GetTransmissionPublisher(ITransmissionService service)
        {
            return new TransmissionPublisher(service, _timeFramePublishHelper);
        }

        public IWheelDrivePublisher GetWheelDrivePublisher(IWheelDriveService service)
        {
            return new WheelDrivePublisher(service, _timeFramePublishHelper);
        }

        public ISteeringPublisher GetSteeringPublisher(ISteeringService service)
        {
            return new SteeringPublisher(service, _timeFramePublishHelper);
        }

        public IGradePublisher GetGradePublisher(IGradeService service)
        {
            return new GradePublisher(service, _timeFramePublishHelper);
        }

        public ICarPublisher GetCarPublisher(ICarService service)
        {
            return new CarPublisher(service, _timeFramePublishHelper);
        }

        public IAssetPublisher GetAssetPublisher(IAssetService service)
        {
            return new AssetPublisher(service);
        }

        public ISubModelPublisher GetSubModelPublisher(ISubModelService service)
        {
            return new SubModelPublisher(service, _timeFramePublishHelper);
        }

        public IGradeEquipmentPublisher GetGradeEquipmentPublisher(IGradeEquipmentService service)
        {
            return new GradeEquipmentPublisher(service, _timeFramePublishHelper);
        }

        public IGradePackPublisher GetGradePackPublisher(IGradePackService service)
        {
            return new GradePackPublisher(service, _timeFramePublishHelper);
        }
    }
}
