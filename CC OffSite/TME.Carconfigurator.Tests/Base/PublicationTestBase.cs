﻿using System;
using System.Linq;
using FakeItEasy;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.PutServices.Interfaces;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Tests.Shared;
using System.Threading.Tasks;
using TME.CarConfigurator.S3.Shared.Result;
using System.Collections.Generic;
using TME.CarConfigurator.S3.Shared.Interfaces;
using IPublicationService = TME.CarConfigurator.S3.PutServices.Interfaces.IPublicationService;

namespace TME.Carconfigurator.Tests.Base
{
    public abstract class PublicationTestBase : TestBase
    {
        protected IPublicationService PublicationService;
        protected ILanguageService PutLanguageService;
        protected CarConfigurator.S3.GetServices.Interfaces.ILanguageService GetLanguageService;
        protected IBodyTypeService BodyTypeService;
        protected IEngineService EngineService;
        protected S3Publisher Publisher;
        protected ISerialiser Serialiser;
        protected IContext Context;
        protected String Brand = "Toyota";
        protected String Country = "BE";
        protected String[] Languages = { "nl", "fr", "de", "en" };

        protected String GuidRegexPattern = @"\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b";


        protected override void Arrange()
        {
            PublicationService = A.Fake<IPublicationService>(x => x.Strict());
            PutLanguageService = A.Fake<ILanguageService>(x => x.Strict());
            GetLanguageService = A.Fake<CarConfigurator.S3.GetServices.Interfaces.ILanguageService>(x => x.Strict());
            BodyTypeService = A.Fake<IBodyTypeService>(x => x.Strict());
            EngineService = A.Fake<IEngineService>(x => x.Strict());

            var successFullTask = Task.FromResult((Result)new Successfull());
            var successFullTasks = Task.FromResult((IEnumerable<Result>)new[] { new Successfull() });

            Serialiser = A.Fake<ISerialiser>();

            Publisher = new S3Publisher(PublicationService, PutLanguageService, GetLanguageService, BodyTypeService, EngineService);
            Context = ContextBuilder.GetDefaultContext(Languages);

            A.CallTo(() => Serialiser.Serialise((Publication)null)).WithAnyArguments().ReturnsLazily(args => args.Arguments.First().GetType().Name);
            A.CallTo(() => PutLanguageService.PutModelsOverviewPerLanguage(null, null)).WithAnyArguments().Returns(successFullTask);
            A.CallTo(() => GetLanguageService.GetLanguages(Context.Brand, Context.Country)).Returns(new Languages());
            A.CallTo(() => PublicationService.PutPublications(null)).WithAnyArguments().Returns(successFullTasks);
            A.CallTo(() => BodyTypeService.PutGenerationBodyTypes(null)).WithAnyArguments().Returns(successFullTasks);
            A.CallTo(() => EngineService.PutGenerationEngines(null)).WithAnyArguments().Returns(successFullTasks);

        }

        protected override void Act()
        {
            var result = Publisher.Publish(Context).Result;
        }
    }
}
