using System;
using System.Collections.Generic;
using FakeItEasy;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Publisher.Enums;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.S3;
using TME.CarConfigurator.Repository.Objects;
using TME.Carconfigurator.Tests.Base;

namespace TME.Carconfigurator.Tests.GivenAS3Publisher
{
    public abstract class ActivatePublicationTestBase : TestBase
    {
        protected IS3Service Service;
        protected IPublisher Publisher;
        protected const string Brand = "Toyota";
        protected const string Country = "BE";
        protected const string Language1 = "nl";
        protected const string Language2 = "fr";
        protected Context Context;
        protected readonly Guid GenerationID = Guid.NewGuid();
        protected readonly Guid ModelID = Guid.NewGuid();
        protected const string ModelNameForLanguage1 = "GenerationName1";
        protected const string ModelNameForLanguage2 = "GenerationName2";

        protected override void Arrange()
        {
            Service = A.Fake<IS3Service>(x => x.Strict());
            var serialiser = A.Fake<IS3Serialiser>();
            Context = new Context(Brand, Country, GenerationID, PublicationDataSubset.Live);

            var contextDataForLanguage1 = new ContextData();
            contextDataForLanguage1.Models.Add(new Model { Name = ModelNameForLanguage1, ID = ModelID });
            contextDataForLanguage1.Generations.Add(new Generation());

            var contextDataForLanguage2 = new ContextData();
            contextDataForLanguage2.Models.Add(new Model { Name = ModelNameForLanguage2, ID = ModelID });
            contextDataForLanguage2.Generations.Add(new Generation());

            Context.ContextData.Add(Language1, contextDataForLanguage1);
            Context.ContextData.Add(Language2, contextDataForLanguage2);

            var timeFrames = new List<TimeFrame> { new TimeFrame(DateTime.MinValue, DateTime.MaxValue, new List<Car>()) };

            Context.TimeFrames.Add(Language1, timeFrames);
            Context.TimeFrames.Add(Language2, timeFrames);

            A.CallTo(() => Service.PutModelsOverview(null, null, null, null)).WithAnyArguments();
            A.CallTo(() => Service.PutObject(null, null)).WithAnyArguments();

            Publisher = new S3Publisher(Service, serialiser);
        }

        protected override void Act()
        {
            Publisher.Publish(Context);
        }

        protected Models GetModel(string modelName)
        {
            return new Models()
            {
                new Model {Name = modelName,ID = ModelID, Publications =
                {
                    new PublicationInfo(new Publication{ID = Guid.NewGuid(),Generation = new Generation()})
                }}
            };
        }
    }
}