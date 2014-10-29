﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Common.Result;
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
        private IGradeEquipmentPublisher _gradeEquipmentPublisher;
        private IColourCombinationPublisher _colourCombinationPublisher;
        private IAssetPublisher _assetPublisher;
        private IGradePackPublisher _gradePackPublisher;

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
            _gradeEquipmentPublisher = A.Fake<IGradeEquipmentPublisher>(x => x.Strict());
            _gradePackPublisher = A.Fake<IGradePackPublisher>(x => x.Strict());
            _colourCombinationPublisher = A.Fake<IColourCombinationPublisher>(x => x.Strict());

            var successFullTask = Task.FromResult((Result)new Successfull());
            var successFullTasks = Task.FromResult((IEnumerable<Result>)new[] { new Successfull() });

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
                .WithAssetPublisher(_assetPublisher)
                .WithSubModelPublisher(_subModelPublisher)
                .WithGradeEquipmentPublisher(_gradeEquipmentPublisher)
                .WithGradePackPublisher(_gradePackPublisher)
                .WithColourCombinationPublisher(_colourCombinationPublisher)
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

                contextBuilder.WithCars(language, cars);

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

            _context = contextBuilder.Build();

            A.CallTo(() => _serialiser.Serialise(null)).WithAnyArguments().ReturnsLazily(args => args.Arguments.First().GetType().Name);
            A.CallTo(() => _serialiser.Serialise(A<Publication>._)).ReturnsLazily(args => SerialisedData);

            A.CallTo(() => _modelPublisher.PublishModelsByLanguage(null, null)).WithAnyArguments().Returns(successFullTask);
            A.CallTo(() => _modelService.GetModelsByLanguage(_context.Brand, _context.Country)).Returns(new Languages());

            A.CallTo(() => _publicationPublisher.PublishPublicationsAsync(_context)).Returns(successFullTasks);
            A.CallTo(() => _bodyTypePublisher.PublishGenerationBodyTypesAsync(_context)).Returns(successFullTasks);
            A.CallTo(() => _enginePublisher.PublishGenerationEnginesAsync(_context)).Returns(successFullTasks);
            A.CallTo(() => _transmissionPublisher.PublishGenerationTransmissionsAsync(_context)).Returns(successFullTasks);
            A.CallTo(() => _wheelDrivePublisher.PublishGenerationWheelDrivesAsync(_context)).Returns(successFullTasks);
            A.CallTo(() => _steeringPublisher.PublishGenerationSteeringsAsync(_context)).Returns(successFullTasks);
            A.CallTo(() => _gradePublisher.PublishGenerationGradesAsync(_context)).Returns(successFullTasks);
            A.CallTo(() => _carPublisher.PublishGenerationCarsAsync(_context)).Returns(successFullTasks);
            A.CallTo(() => _assetPublisher.PublishAssetsAsync(_context)).Returns(successFullTasks);
            A.CallTo(() => _assetPublisher.PublishCarAssetsAsync(_context)).Returns(successFullTasks);
            A.CallTo(() => _subModelPublisher.PublishGenerationSubModelsAsync(_context)).Returns(successFullTasks);
            A.CallTo(() => _gradeEquipmentPublisher.PublishAsync(_context)).Returns(successFullTasks);
            A.CallTo(() => _gradePackPublisher.PublishAsync(_context)).Returns(successFullTasks);
            A.CallTo(() => _colourCombinationPublisher.PublishGenerationColourCombinationsAsync(_context)).Returns(successFullTasks);
        }

        protected override void Act()
        {
            var result = _publisher.PublishAsync(_context).Result;
        }

        [Fact]
        public void ThenAPublicationShouldBePublished()
        {
            A.CallTo(() => _publicationPublisher.PublishPublicationsAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenAPublishGenerationBodyTypesShouldHappen()
        {
            A.CallTo(() => _bodyTypePublisher.PublishGenerationBodyTypesAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenAPublishGenerationEnginesShouldHappen()
        {
            A.CallTo(() => _enginePublisher.PublishGenerationEnginesAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenAPublishGenerationTransmissionsShouldHappen()
        {
            A.CallTo(() => _transmissionPublisher.PublishGenerationTransmissionsAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenAPublishGenerationWheelDrivesShouldHappen()
        {
            A.CallTo(() => _wheelDrivePublisher.PublishGenerationWheelDrivesAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenAPublishGenerationSteeringsShouldHappen()
        {
            A.CallTo(() => _steeringPublisher.PublishGenerationSteeringsAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenAPublishGenerationGradesShouldHappen()
        {
            A.CallTo(() => _gradePublisher.PublishGenerationGradesAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenAPublishGenerationCarsShouldHappen()
        {
            A.CallTo(() => _carPublisher.PublishGenerationCarsAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishGenerationAssets()
        {
            A.CallTo(() => _assetPublisher.PublishAssetsAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishCarAssets()
        {
            A.CallTo(() => _assetPublisher.PublishCarAssetsAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishGenerationSubModels()
        {
            A.CallTo(() => _subModelPublisher.PublishGenerationSubModelsAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishGenerationGradeEquipments()
        {
            A.CallTo(() => _gradeEquipmentPublisher.PublishAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishTheGradePacks()
        {
            A.CallTo(() => _gradePackPublisher.PublishAsync(_context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishGenerationColourCombinations()
        {
            A.CallTo(() => _colourCombinationPublisher.PublishGenerationColourCombinationsAsync(_context))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
