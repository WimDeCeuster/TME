using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.Carconfigurator.Tests.Builders
{
    public class PublisherBuilder
    {
        private IPublicationPublisher _publicationPublisher = A.Fake<IPublicationPublisher>();
        private IModelPublisher _modelPublisher = A.Fake<IModelPublisher>();
        private IModelService _modelService = A.Fake<IModelService>();
        private IBodyTypePublisher _bodyTypePublisher = A.Fake<IBodyTypePublisher>();
        private IEnginePublisher _enginePublisher = A.Fake<IEnginePublisher>();
        private ITransmissionPublisher _transmissionPublisher = A.Fake<ITransmissionPublisher>();
        private IWheelDrivePublisher _wheelDrivePublisher = A.Fake<IWheelDrivePublisher>();
        private ISteeringPublisher _steeringPublisher = A.Fake<ISteeringPublisher>();
        private IGradePublisher _gradePublisher = A.Fake<IGradePublisher>();
        private ICarPublisher _carPublisher = A.Fake<ICarPublisher>();
        private IAssetPublisher _assetPublisher = A.Fake<IAssetPublisher>();
        private ISubModelPublisher _subModelPublisher = A.Fake<ISubModelPublisher>();
        private IGradeEquipmentPublisher _gradeEquipmentPublisher = A.Fake<IGradeEquipmentPublisher>();
        private IGradePackPublisher _gradePackPublisher = A.Fake<IGradePackPublisher>();

        public PublisherBuilder WithPublicationPublisher(IPublicationPublisher publicationPublisher)
        {
            _publicationPublisher = publicationPublisher;

            return this;
        }

        public PublisherBuilder WithModelPublisher(IModelPublisher modelPublisher)
        {
            _modelPublisher = modelPublisher;

            return this;
        }
        
        public PublisherBuilder WithSubModelPublisher(ISubModelPublisher subModelPublisher)
        {
            _subModelPublisher = subModelPublisher;

            return this;
        }

        public PublisherBuilder WithModelService(IModelService modelService)
        {
            _modelService = modelService;

            return this;
        }

        public PublisherBuilder WithBodyTypePublisher(IBodyTypePublisher bodyTypePublisher)
        {
            _bodyTypePublisher = bodyTypePublisher;

            return this;
        }

        public PublisherBuilder WithEnginePublisher(IEnginePublisher enginePublisher)
        {
            _enginePublisher = enginePublisher;

            return this;
        }

        public PublisherBuilder WithTransmissionPublisher(ITransmissionPublisher transmissionPublisher)
        {
            _transmissionPublisher = transmissionPublisher;

            return this;
        }

        public PublisherBuilder WithWheelDrivePublisher(IWheelDrivePublisher wheelDrivePublisher)
        {
            _wheelDrivePublisher = wheelDrivePublisher;

            return this;
        }

        public PublisherBuilder WithSteeringPublisher(ISteeringPublisher steeringPublisher)
        {
            _steeringPublisher = steeringPublisher;

            return this;
        }

        public PublisherBuilder WithGradePublisher(IGradePublisher gradePublisher)
        {
            _gradePublisher = gradePublisher;

            return this;
        }

        public PublisherBuilder WithCarPublisher(ICarPublisher carPublisher)
        {
            _carPublisher = carPublisher;

            return this;
        }

        public PublisherBuilder WithAssetPublisher(IAssetPublisher assetPublisher)
        {
            _assetPublisher = assetPublisher;

            return this;
        }

        public PublisherBuilder WithGradeEquipmentPublisher(IGradeEquipmentPublisher gradeEquipmentPublisher)
        {
            _gradeEquipmentPublisher = gradeEquipmentPublisher;

            return this;
        }

        public Publisher Build()
        {
            return new Publisher(
                _publicationPublisher,
                _modelPublisher,
                _modelService,
                _bodyTypePublisher,
                _enginePublisher,
                _transmissionPublisher,
                _wheelDrivePublisher,
                _steeringPublisher,
                _gradePublisher,
                _carPublisher,
                _assetPublisher,
                _subModelPublisher,
                _gradeEquipmentPublisher,
                _gradePackPublisher
                );
        }

        public PublisherBuilder WithGradePackPublisher(IGradePackPublisher gradePackPublisher)
        {
            _gradePackPublisher = gradePackPublisher;

            return this;
        }
    }
}
