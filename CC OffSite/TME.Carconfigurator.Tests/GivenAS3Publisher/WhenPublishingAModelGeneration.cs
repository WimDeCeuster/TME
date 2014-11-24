using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Common.Interfaces;

using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAS3Publisher
{
    public class WhenPublishingAModelGeneration : TestBase
    {
        private readonly String[] _languages = { "nl", "fr", "de", "en" };

        private const string SerialisedData = "aaa";
        private IPublisher _publisher;
        private ISerialiser _serialiser;
        private IContext _context;
        private CarConfigurator.QueryServices.IModelService _modelService;

        private IPublicationPublisher _publicationPublisher;
        private IModelPublisher _modelPublisher;
        private IBodyTypePublisher _bodyTypePublisher;
        private IEnginePublisher _enginePublisher;
        private ITransmissionPublisher _transmissionPublisher;
        private IWheelDrivePublisher _wheelDrivePublisher;
        private ISteeringPublisher _steeringPublisher;
        private IGradePublisher _gradePublisher;
        private ICarPublisher _carPublisher;
        private ISubModelPublisher _subModelPublisher;
        private IEquipmentPublisher _equipmentPublisher;
        private ISpecificationsPublisher _specificationsPublisher;
        private IColourPublisher _colourCombinationPublisher;
        private IAssetPublisher _assetPublisher;
        private ICarPartPublisher _carPartPublisher;
        private ICarEquipmentPublisher _carEquipmentPublisher;
        private IPackPublisher _packPublisher;

        protected override void Arrange()
        {
            _publicationPublisher = A.Fake<IPublicationPublisher>(x => x.Strict());
            _modelPublisher = A.Fake<IModelPublisher>(x => x.Strict());
            _modelService = A.Fake<CarConfigurator.QueryServices.IModelService>(x => x.Strict());
            _bodyTypePublisher = A.Fake<IBodyTypePublisher>(x => x.Strict());
            _enginePublisher = A.Fake<IEnginePublisher>(x => x.Strict());
            _transmissionPublisher = A.Fake<ITransmissionPublisher>(x => x.Strict());
            _wheelDrivePublisher = A.Fake<IWheelDrivePublisher>(x => x.Strict());
            _steeringPublisher = A.Fake<ISteeringPublisher>(x => x.Strict());
            _gradePublisher = A.Fake<IGradePublisher>(x => x.Strict());
            _carPublisher = A.Fake<ICarPublisher>(x => x.Strict());
            _assetPublisher = A.Fake<IAssetPublisher>(x => x.Strict());
            _subModelPublisher = A.Fake<ISubModelPublisher>(x => x.Strict());
            _equipmentPublisher = A.Fake<IEquipmentPublisher>(x => x.Strict());
            _specificationsPublisher = A.Fake<ISpecificationsPublisher>(x => x.Strict());
            _colourCombinationPublisher = A.Fake<IColourPublisher>(x => x.Strict());
            _carPartPublisher = A.Fake<ICarPartPublisher>(x => x.Strict());
            _carEquipmentPublisher = A.Fake<ICarEquipmentPublisher>(x => x.Strict());
            _packPublisher = A.Fake<IPackPublisher>(x => x.Strict());

            _serialiser = A.Fake<ISerialiser>();

            _publisher = new PublisherBuilder()
                .WithPublicationPublisher(_publicationPublisher)
                .WithModelPublisher(_modelPublisher)
                .WithModelService(_modelService)
                .WithBodyTypePublisher(_bodyTypePublisher)
                .WithEnginePublisher(_enginePublisher)
                .WithTransmissionPublisher(_transmissionPublisher)
                .WithWheelDrivePublisher(_wheelDrivePublisher)
                .WithSteeringPublisher(_steeringPublisher)
                .WithGradePublisher(_gradePublisher)
                .WithCarPublisher(_carPublisher)
                .WithCarEquipmentPublisher(_carEquipmentPublisher)
                .WithAssetPublisher(_assetPublisher)
                .WithSubModelPublisher(_subModelPublisher)
                .WithEquipmentPublisher(_equipmentPublisher)
                .WithSpecificationsPublisher(_specificationsPublisher)
                .WithPackPublisher(_packPublisher)
                .WithColourCombinationPublisher(_colourCombinationPublisher)
                .WithCarPartPublisher(_carPartPublisher)
                .Build();

            var contextBuilder = new ContextBuilder()
                       .WithBrand("Toyota")
                       .WithCountry("DE")
                       .WithDataSubset(PublicationDataSubset.Live)
                       .WithLanguages(_languages);

            foreach (var language in _languages)
            {
                contextBuilder.WithGeneration(language, new GenerationBuilder().Build());
                var cars = new[] {
                    new CarBuilder().Build(),
                    new CarBuilder().Build(),
                    new CarBuilder().Build()
                };

                var timeFrames = new[] {
                    new TimeFrameBuilder()
                        .WithDateRange(new DateTime(2014, 1, 1), 
                                       new DateTime(2014, 4, 4))
                        .WithCars(cars.Take(1))
                        .Build(),
            
                    new TimeFrameBuilder()
                        .WithDateRange(new DateTime(2014, 4, 4), 
                                       new DateTime(2014, 8, 8))
                        .WithCars(cars.Take(2))
                        .Build(),
            
                    new TimeFrameBuilder()
                        .WithDateRange(new DateTime(2014, 8, 8), 
                                       new DateTime(2014, 12, 12))
                        .WithCars(cars.Skip(1).Take(2))
                        .Build()
                };

                contextBuilder.WithTimeFrames(language, timeFrames);
                contextBuilder.WithModel(language, new Model());
            }

            var task = Task.Run(() => { });
            _context = contextBuilder.Build();

            A.CallTo(() => _serialiser.Serialise(null)).WithAnyArguments().ReturnsLazily(args => args.Arguments.First().GetType().Name);
            A.CallTo(() => _serialiser.Serialise(A<Publication>._)).ReturnsLazily(args => SerialisedData);

            A.CallTo(() => _modelPublisher.PublishModelsByLanguage(null, null)).WithAnyArguments().Returns(task);
            A.CallTo(() => _modelService.GetModelsByLanguage(_context.Brand, _context.Country)).Returns(new Languages());

            A.CallTo(() => _publicationPublisher.PublishPublicationsAsync(_context)).Returns(task);
            A.CallTo(() => _bodyTypePublisher.PublishGenerationBodyTypesAsync(_context)).Returns(task);
            A.CallTo(() => _enginePublisher.PublishGenerationEnginesAsync(_context)).Returns(task);
            A.CallTo(() => _transmissionPublisher.PublishGenerationTransmissionsAsync(_context)).Returns(task);
            A.CallTo(() => _wheelDrivePublisher.PublishGenerationWheelDrivesAsync(_context)).Returns(task);
            A.CallTo(() => _steeringPublisher.PublishGenerationSteeringsAsync(_context)).Returns(task);
            A.CallTo(() => _gradePublisher.PublishGenerationGradesAsync(_context)).Returns(task);
            A.CallTo(() => _gradePublisher.PublishSubModelGradesAsync(_context)).Returns(task);
            A.CallTo(() => _carPublisher.PublishGenerationCarsAsync(_context)).Returns(task);
            A.CallTo(() => _assetPublisher.PublishAsync(_context)).Returns(task);
            A.CallTo(() => _subModelPublisher.PublishGenerationSubModelsAsync(_context)).Returns(task);
            A.CallTo(() => _equipmentPublisher.PublishAsync(_context)).Returns(task);
            A.CallTo(() => _equipmentPublisher.PublishCategoriesAsync(_context)).Returns(task);
            A.CallTo(() => _equipmentPublisher.PublishSubModelGradeEquipmentAsync(_context)).Returns(task);
            A.CallTo(() => _specificationsPublisher.PublishCategoriesAsync(_context)).Returns(task);
            A.CallTo(() => _specificationsPublisher.PublishCarTechnicalSpecificationsAsync(_context)).Returns(task);
            A.CallTo(() => _packPublisher.PublishGradePacksAsync(_context)).Returns(task);
            A.CallTo(() => _packPublisher.PublishCarPacksAsync(_context)).Returns(task);
            A.CallTo(() => _packPublisher.PublishSubModelGradePacksAsync(_context)).Returns(task);
            A.CallTo(() => _colourCombinationPublisher.PublishGenerationColourCombinations(_context)).Returns(task);
            A.CallTo(() => _colourCombinationPublisher.PublishCarColourCombinations(_context)).Returns(task);
            A.CallTo(() => _carPartPublisher.PublishCarPartsAsync(_context)).Returns(task);
            A.CallTo(() => _carEquipmentPublisher.PublishCarEquipmentAsync(_context)).Returns(task);
        }

        protected override void Act()
        {
            _publisher.PublishAsync(_context).Wait();
        }

        [Fact]
        public void ThenItShouldPublishAPublication()
        {
            A.CallTo(() => _publicationPublisher.PublishPublicationsAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishCarEquipment()
        {
            A.CallTo(() => _carEquipmentPublisher.PublishCarEquipmentAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }
        [Fact]
        public void ThenItShouldPublishCarTechnicalSpecifications()
        {
            A.CallTo(() => _specificationsPublisher.PublishCarTechnicalSpecificationsAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }
        [Fact]
        public void ThenItShouldPublishGenerationBodyTypes()
        {
            A.CallTo(() => _bodyTypePublisher.PublishGenerationBodyTypesAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishCarParts()
        {
            A.CallTo(() => _carPartPublisher.PublishCarPartsAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishGenerationEngines()
        {
            A.CallTo(() => _enginePublisher.PublishGenerationEnginesAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishGenerationTransmissions()
        {
            A.CallTo(() => _transmissionPublisher.PublishGenerationTransmissionsAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishGenerationWheelDrives()
        {
            A.CallTo(() => _wheelDrivePublisher.PublishGenerationWheelDrivesAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishGenerationSteerings()
        {
            A.CallTo(() => _steeringPublisher.PublishGenerationSteeringsAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishGenerationGrades()
        {
            A.CallTo(() => _gradePublisher.PublishGenerationGradesAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishGenerationCars()
        {
            A.CallTo(() => _carPublisher.PublishGenerationCarsAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishAllAssets()
        {
            A.CallTo(() => _assetPublisher.PublishAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishGenerationSubModels()
        {
            A.CallTo(() => _subModelPublisher.PublishGenerationSubModelsAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishGenerationGradeEquipments()
        {
            A.CallTo(() => _equipmentPublisher.PublishAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishTheGradePacks()
        {
            A.CallTo(() => _packPublisher.PublishGradePacksAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishGenerationColourCombinations()
        {
            A.CallTo(() => _colourCombinationPublisher.PublishGenerationColourCombinations(_context))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishEquipmentCategories()
        {
            A.CallTo(() => _equipmentPublisher.PublishCategoriesAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishSpecificationCategories()
        {
            A.CallTo(() => _specificationsPublisher.PublishCategoriesAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishCarColourCombinations()
        {
            A.CallTo(() => _colourCombinationPublisher.PublishCarColourCombinations(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishSubModelGradePacks()
        {
            A.CallTo(() => _packPublisher.PublishSubModelGradePacksAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishCarPacks()
        {
            A.CallTo(() => _packPublisher.PublishCarPacksAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
