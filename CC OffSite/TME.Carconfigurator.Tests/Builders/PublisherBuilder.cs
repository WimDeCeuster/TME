﻿using FakeItEasy;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.QueryServices;

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
        private IEquipmentPublisher _equipmentPublisher = A.Fake<IEquipmentPublisher>();
        private IColourPublisher _colourCombinationPublisher = A.Fake<IColourPublisher>();
        private IPackPublisher _packPublisher = A.Fake<IPackPublisher>();
        private ISpecificationsPublisher _specificationsPublisher = A.Fake<ISpecificationsPublisher>();
        private ICarPartPublisher _carPartPublisher = A.Fake<ICarPartPublisher>();
        private IRulePublisher _rulePublisher = A.Fake<IRulePublisher>();

        public PublisherBuilder WithPublicationPublisher(IPublicationPublisher publicationPublisher)
        {
            _publicationPublisher = publicationPublisher;

            return this;
        }

        public PublisherBuilder WithColourCombinationPublisher(IColourPublisher colourCombinationPublisher)
        {
            _colourCombinationPublisher = colourCombinationPublisher;
            return this;
        }

        public PublisherBuilder WithCarPartPublisher(ICarPartPublisher carPartPublisher)
        {
            _carPartPublisher = carPartPublisher;
            return this;
        }

        public PublisherBuilder WithRulePublisher(IRulePublisher rulePublisher)
        {
            _rulePublisher = rulePublisher;
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

        public PublisherBuilder WithEquipmentPublisher(IEquipmentPublisher equipmentPublisher)
        {
            _equipmentPublisher = equipmentPublisher;

            return this;
        }

        public PublisherBuilder WithSpecificationsPublisher(ISpecificationsPublisher specificationsPublisher)
        {
            _specificationsPublisher = specificationsPublisher;

            return this;
        }

        public PublisherBuilder WithPackPublisher(IPackPublisher packPublisher)
        {
            _packPublisher = packPublisher;

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
                _equipmentPublisher,
                _specificationsPublisher,
                _packPublisher,
                _colourCombinationPublisher,
                _carPartPublisher,
                _rulePublisher);
        }
    }
}
