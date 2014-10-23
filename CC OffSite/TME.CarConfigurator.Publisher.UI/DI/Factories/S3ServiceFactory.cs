using Spring.Context.Support;
using System;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.UI.DI.Interfaces;
using TME.CarConfigurator.S3.CommandServices;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.Publisher.UI.DI.Factories
{
    public class S3ServiceFactory : IServiceFactory
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

        public QueryServices.IModelService GetGetModelService(String environment, PublicationDataSubset dataSubset)
        {
            var service = GetService(environment, dataSubset);

            return new S3.QueryServices.ModelService(_serialiser, service, _keyManager);
        }

        public IModelService GetPutModelService(String environment, PublicationDataSubset dataSubset)
        {
            var service = GetService(environment, dataSubset);

            return new ModelService(service, _serialiser, _keyManager);
        }

        public IPublicationService GetPublicationService(String environment, PublicationDataSubset dataSubset)
        {
            var service = GetService(environment, dataSubset);

            return new PublicationService(service, _serialiser, _keyManager);
        }

        public IBodyTypeService GetBodyTypeService(String environment, PublicationDataSubset dataSubset)
        {
            var service = GetService(environment, dataSubset);

            return new BodyTypeService(service, _serialiser, _keyManager);
        }

        public IEngineService GetEngineService(String environment, PublicationDataSubset dataSubset)
        {
            var service = GetService(environment, dataSubset);

            return new EngineService(service, _serialiser, _keyManager);
        }

        public ITransmissionService GetTransmissionService(String environment, PublicationDataSubset dataSubset)
        {
            var service = GetService(environment, dataSubset);

            return new TransmissionService(service, _serialiser, _keyManager);
        }

        public IWheelDriveService GetWheelDriveService(String environment, PublicationDataSubset dataSubset)
        {
            var service = GetService(environment, dataSubset);

            return new WheelDriveService(service, _serialiser, _keyManager);
        }

        public ISteeringService GetSteeringService(String environment, PublicationDataSubset dataSubset)
        {
            var service = GetService(environment, dataSubset);

            return new SteeringService(service, _serialiser, _keyManager);
        }

        public IGradeService GetGradeService(String environment, PublicationDataSubset dataSubset)
        {
            var service = GetService(environment, dataSubset);

            return new GradeService(service, _serialiser, _keyManager);
        }

        public ICarService GetCarService(String environment, PublicationDataSubset dataSubset)
        {
            var service = GetService(environment, dataSubset);

            return new CarService(service, _serialiser, _keyManager);
        }

        public IAssetService GetAssetService(String environment, PublicationDataSubset dataSubset)
        {
            var service = GetService(environment, dataSubset);

            return new AssetsService(service, _serialiser, _keyManager);
        }

        public ISubModelService GetSubModelService(String environment, PublicationDataSubset dataSubset)
        {
            var service = GetService(environment, dataSubset);

            return new SubModelService(service, _serialiser, _keyManager);
        }

        public IGradeEquipmentService GetGradeEquipmentService(String environment, PublicationDataSubset dataSubset)
        {
            var service = GetService(environment, dataSubset);

            return new GradeEquipmentService(service, _serialiser, _keyManager);
        }
    }
}
