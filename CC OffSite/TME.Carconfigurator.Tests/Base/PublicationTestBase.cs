using System;
using System.Linq;
using FakeItEasy;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Tests.Shared;
using System.Threading.Tasks;
using TME.CarConfigurator.S3.Shared.Result;
using System.Collections.Generic;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Publisher.S3;

namespace TME.Carconfigurator.Tests.Base
{
    public abstract class PublicationTestBase : TestBase
    {
        protected IS3PublicationService PublicationService;
        protected IS3LanguageService LanguageService;
        protected IS3BodyTypeService BodyTypeService; 
        protected S3Publisher Publisher;
        protected ISerialiser Serialiser;
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
            var successFullTasks = Task.FromResult((IEnumerable<Result>)new[] { new Successfull() });

            Serialiser = A.Fake<ISerialiser>();

            Publisher = new S3Publisher(PublicationService, LanguageService, BodyTypeService);
            Context = ContextBuilder.GetDefaultContext(Languages);

            A.CallTo(() => Serialiser.Serialise((Publication)null)).WithAnyArguments().ReturnsLazily(args => args.Arguments.First().GetType().Name);
            A.CallTo(() => LanguageService.PutModelsOverviewPerLanguage(null, null)).WithAnyArguments().Returns(successFullTask);
            A.CallTo(() => LanguageService.GetModelsOverviewPerLanguage(Context)).Returns(new Languages());
            A.CallTo(() => PublicationService.PutPublications(null)).WithAnyArguments().Returns(successFullTasks);
            A.CallTo(() => BodyTypeService.PutGenerationBodyTypes(null)).WithAnyArguments().Returns(successFullTasks);

        }

        protected override void Act()
        {
            var result = Publisher.Publish(Context).Result;
        }
    }
}
