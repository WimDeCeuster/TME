using System;
using System.Linq;
using FakeItEasy;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Tests.Shared;
using System.Threading.Tasks;
using TME.CarConfigurator.S3.Shared.Result;
using System.Collections.Generic;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Tests.Shared.TestBuilders;

namespace TME.Carconfigurator.Tests.Base
{
    public abstract class PublicationTestBase : TestBase
    {
        protected IPublicationPublisher PublicationPublisher;
        protected IModelPublisher PutModelPublisher;
        protected CarConfigurator.QueryServices.IModelService GetModelService;
        protected IBodyTypePublisher BodyTypePublisher;
        protected IEnginePublisher EnginePublisher;
        protected ITransmissionPublisher TransmissionPublisher;
        protected IWheelDrivePublisher WheelDrivePublisher;
        protected ISteeringPublisher SteeringPublisher;
        protected IGradePublisher GradePublisher;
        protected ICarPublisher CarPublisher;
        protected IAssetPublisher AssetPublisher;
        protected Publisher Publisher;
        protected ISerialiser Serialiser;
        protected IContext Context;
        protected String Brand = "Toyota";
        protected String Country = "BE";
        protected String[] Languages = { "nl", "fr", "de", "en" };

        protected String GuidRegexPattern = @"\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b";


        protected override void Arrange()
        {
            PublicationPublisher = A.Fake<IPublicationPublisher>(x => x.Strict());
            PutModelPublisher = A.Fake<IModelPublisher>(x => x.Strict());
            GetModelService = A.Fake<CarConfigurator.QueryServices.IModelService>(x => x.Strict());
            BodyTypePublisher = A.Fake<IBodyTypePublisher>(x => x.Strict());
            EnginePublisher = A.Fake<IEnginePublisher>(x => x.Strict());
            TransmissionPublisher = A.Fake<ITransmissionPublisher>(x => x.Strict());
            WheelDrivePublisher = A.Fake<IWheelDrivePublisher>(x => x.Strict());
            SteeringPublisher = A.Fake<ISteeringPublisher>(x => x.Strict());
            GradePublisher = A.Fake<IGradePublisher>(x => x.Strict());
            CarPublisher = A.Fake<ICarPublisher>(x => x.Strict());
            AssetPublisher = A.Fake<IAssetPublisher>(x => x.Strict());

            var successFullTask = Task.FromResult((Result)new Successfull());
            var successFullTasks = Task.FromResult((IEnumerable<Result>)new[] { new Successfull() });

            Serialiser = A.Fake<ISerialiser>();

            Publisher = new PublisherBuilder()
                .WithPublicationPublisher(PublicationPublisher)
                .WithModelPublisher(PutModelPublisher)
                .WithModelService(GetModelService)
                .WithBodyTypePublisher(BodyTypePublisher)
                .WithEnginePublisher(EnginePublisher)
                .WithTransmissionPublisher(TransmissionPublisher)
                .WithWheelDrivePublisher(WheelDrivePublisher)
                .WithSteeringPublisher(SteeringPublisher)
                .WithGradePublisher(GradePublisher)
                .WithCarPublisher(CarPublisher)
                .WithAssetPublisher(AssetPublisher)
                .Build();

             var contextBuilder = new ContextBuilder()
                        .WithBrand("Toyota")
                        .WithCountry("DE")
                        .WithDataSubset(PublicationDataSubset.Live)
                        .WithLanguages(Languages);

            foreach (var language in Languages)
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

            Context = contextBuilder.Build();

            A.CallTo(() => Serialiser.Serialise((Publication)null)).WithAnyArguments().ReturnsLazily(args => args.Arguments.First().GetType().Name);
            A.CallTo(() => PutModelPublisher.PublishModelsByLanguage(null, null)).WithAnyArguments().Returns(successFullTask);
            A.CallTo(() => GetModelService.GetModelsByLanguage(Context.Brand, Context.Country)).Returns(new Languages());
            A.CallTo(() => PublicationPublisher.PublishPublications(null)).WithAnyArguments().Returns(successFullTasks);
            A.CallTo(() => BodyTypePublisher.PublishGenerationBodyTypes(null)).WithAnyArguments().Returns(successFullTasks);
            A.CallTo(() => EnginePublisher.PublishGenerationEngines(null)).WithAnyArguments().Returns(successFullTasks);
            A.CallTo(() => TransmissionPublisher.PublishGenerationTransmissions(null)).WithAnyArguments().Returns(successFullTasks);
            A.CallTo(() => WheelDrivePublisher.PublishGenerationWheelDrives(null)).WithAnyArguments().Returns(successFullTasks);
            A.CallTo(() => SteeringPublisher.PublishGenerationSteerings(null)).WithAnyArguments().Returns(successFullTasks);
            A.CallTo(() => GradePublisher.PublishGenerationGrades(null)).WithAnyArguments().Returns(successFullTasks);
            A.CallTo(() => CarPublisher.PublishGenerationCars(null)).WithAnyArguments().Returns(successFullTasks);
            A.CallTo(() => AssetPublisher.PublishAssets(null)).WithAnyArguments().Returns(successFullTasks);

        }

        protected override void Act()
        {
            var result = Publisher.Publish(Context).Result;
        }
    }
}
