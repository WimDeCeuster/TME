using System;
using System.Linq;
using FakeItEasy;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.S3;
using TME.CarConfigurator.Repository.Objects;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Tests.Shared;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Enums.Result;

namespace TME.Carconfigurator.Tests.Base
{
    public abstract class PublicationTestBase : TestBase
    {
        protected IS3PublicationService PublicationService;
        protected IS3LanguageService LanguageService;
        protected IS3BodyTypeService BodyTypeService; 
        protected S3Publisher Publisher;
        protected IS3Serialiser Serialiser;
        protected IContext Context;
        protected String Brand = "Toyota";
        protected String Country = "BE";
        protected String[] Languages = { "nl", "fr", "de", "en" };

        protected String GuidRegexPattern = @"\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b";


        protected override void Arrange()
        {
            PublicationService = A.Fake<IS3PublicationService>(x => x.Strict());
            LanguageService = A.Fake<IS3LanguageService>(x => x.Strict());
            BodyTypeService = A.Fake<IS3BodyTypeService>(x => x.Strict());

            var successFullTask = Task.FromResult((Result)new Successfull());

            Serialiser = A.Fake<IS3Serialiser>();

            A.CallTo(() => Serialiser.Serialise((Publication)null)).WithAnyArguments().ReturnsLazily(args => args.Arguments.First().GetType().Name);
            A.CallTo(() => LanguageService.PutModelsOverviewPerLanguage(null)).WithAnyArguments().Returns(successFullTask);
            A.CallTo(() => PublicationService.PutPublication(null)).WithAnyArguments().Returns(successFullTask);
            A.CallTo(() => BodyTypeService.PutGenerationBodyTypes(null, null, null)).WithAnyArguments().Returns(successFullTask);

            Publisher = new S3Publisher(PublicationService, LanguageService, BodyTypeService);
            Context = ContextBuilder.GetDefaultContext(Languages);
        }

        protected override void Act()
        {
            Publisher.Publish(Context);
        }
    }
}
