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
using TME.CarConfigurator.S3.Publisher;

namespace TME.CarConfigurator.Publisher.UI.DI.Factories
{
    public class S3PublisherFactory : IPublisherFactory
    {
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
            return new BodyTypePublisher(service);
        }

        public IEnginePublisher GetEnginePublisher(IEngineService service)
        {
            return new EnginePublisher(service);
        }

        public ITransmissionPublisher GetTransmissionPublisher(ITransmissionService service)
        {
            return new TransmissionPublisher(service);
        }

        public IWheelDrivePublisher GetWheelDrivePublisher(IWheelDriveService service)
        {
            return new WheelDrivePublisher(service);
        }

        public ISteeringPublisher GetSteeringPublisher(ISteeringService service)
        {
            return new SteeringPublisher(service);
        }

        public IGradePublisher GetGradePublisher(IGradeService service)
        {
            return new GradePublisher(service);
        }

        public ICarPublisher GetCarPublisher(ICarService service)
        {
            return new CarPublisher(service);
        }

        public IAssetPublisher GetAssetPublisher(IAssetService service)
        {
            return new AssetPublisher(service);
        }
    }
}
