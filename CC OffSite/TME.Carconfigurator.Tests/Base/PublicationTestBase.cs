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

namespace TME.Carconfigurator.Tests.Base
{
    public abstract class PublicationTestBase : TestBase
    {
        protected IPublicationPublisher PublicationPublisher;
        protected IModelPublisher PutModelPublisher;
        protected CarConfigurator.QueryServices.IModelService GetModelService;
        protected IBodyTypePublisher BodyTypePublisher;
        protected IEnginePublisher EnginePublisher;
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
                .WithCarPublisher(CarPublisher)
                .WithAssetPublisher(AssetPublisher)
                .Build();

            Context = ContextBuilder.GetDefaultContext(Languages);

            A.CallTo(() => Serialiser.Serialise((Publication)null)).WithAnyArguments().ReturnsLazily(args => args.Arguments.First().GetType().Name);
            A.CallTo(() => PutModelPublisher.PublishModelsByLanguage(null, null)).WithAnyArguments().Returns(successFullTask);
            A.CallTo(() => GetModelService.GetModelsByLanguage(Context.Brand, Context.Country)).Returns(new Languages());
            A.CallTo(() => PublicationPublisher.PublishPublications(null)).WithAnyArguments().Returns(successFullTasks);
            A.CallTo(() => BodyTypePublisher.PublishGenerationBodyTypes(null)).WithAnyArguments().Returns(successFullTasks);
            A.CallTo(() => EnginePublisher.PublishGenerationEngines(null)).WithAnyArguments().Returns(successFullTasks);
            A.CallTo(() => CarPublisher.PublishGenerationCars(null)).WithAnyArguments().Returns(successFullTasks);
            A.CallTo(() => AssetPublisher.PublishAssets(null)).WithAnyArguments().Returns(successFullTasks);

        }

        protected override void Act()
        {
            var result = Publisher.Publish(Context).Result;
        }
    }
}
