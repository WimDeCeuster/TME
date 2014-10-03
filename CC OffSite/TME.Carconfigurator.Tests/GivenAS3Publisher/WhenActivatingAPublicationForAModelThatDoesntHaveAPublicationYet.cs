using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Publisher.Enums;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.S3;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Enums;
using TME.Carconfigurator.Tests.Base;
using Xunit;
using Car = TME.CarConfigurator.Repository.Objects.Car;
using Model = TME.CarConfigurator.Repository.Objects.Model;
using Models = TME.CarConfigurator.Repository.Objects.Models;

namespace TME.Carconfigurator.Tests.GivenAS3Publisher
{
    public class WhenActivatingAPublicationForAModelThatDoesntHaveAPublicationYet : TestBase
    {
        private IS3Service _service;
        private S3Publisher _publisher;
        private const string Brand = "Toyota";
        private const string Country = "BE";
        private const string Language1 = "nl";
        private const string Language2 = "fr";
        private Context _context;
        private readonly Guid _generationID = Guid.NewGuid();
        private Models _models; //new repo models
        private const string ModelNameForLanguage1 = "GenerationName1";
        private const string ModelNameForLanguage2 = "GenerationName2";

        protected override void Arrange()
        {
            _service = A.Fake<IS3Service>();
            var serialiser = A.Fake<IS3Serialiser>();
            _context = new Context(Brand, Country, _generationID, PublicationDataSubset.Live);
            
            var contextDataForLanguage1 = new ContextData();
            contextDataForLanguage1.Models.Add(new Model {Name = ModelNameForLanguage1});
            contextDataForLanguage1.Generations.Add(new Generation());
            
            var contextDataForLanguage2 = new ContextData();
            contextDataForLanguage2.Models.Add(new Model {Name = ModelNameForLanguage2});
            contextDataForLanguage2.Generations.Add(new Generation());

            _context.ContextData.Add(Language1,contextDataForLanguage1);
            _context.ContextData.Add(Language2,contextDataForLanguage2);

            var timeFrames = new List<TimeFrame>{new TimeFrame(DateTime.MinValue, DateTime.MaxValue,new List<Car>())};

            _context.TimeFrames.Add(Language1,timeFrames);
            _context.TimeFrames.Add(Language2,timeFrames);

            _models = new Models();
            A.CallTo(() => _service.GetModelsOverview(Brand, Country, Language1)).Returns(_models);

            _publisher = new S3Publisher(_service, serialiser);
        }

        protected override void Act()
        {
            _publisher.Publish(_context);
        }

        [Fact]
        public void ThenItShouldUploadCorrectDataForLanguage1()
        {
            A.CallTo(() => _service.PutModelsOverview(Brand, Country, Language1, null))
                .WhenArgumentsMatch(args =>
                {
                    var models = ((Models)args[3]);
                    return ShouldContainModelWithActivatedPublication(models,ModelNameForLanguage1);
                })
                .MustHaveHappened(Repeated.Exactly.Once);
        }
        
        [Fact]
        public void ThenItShouldUploadCorrectDataForLanguage2()
        {
            A.CallTo(() => _service.PutModelsOverview(Brand, Country, Language2, null))
                .WhenArgumentsMatch(args =>
                {
                    var models = ((Models)args[3]);
                    return ShouldContainModelWithActivatedPublication(models,ModelNameForLanguage2);
                })
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private bool ShouldContainModelWithActivatedPublication(IEnumerable<Model> models, string modelName)
        {
            var model = models.SingleOrDefault(m => m.Name.Equals(modelName));
            return model != null && model.Publications.Count == 1 && model.Publications.All(p => p.State == PublicationState.Activated);
        }
    }
}