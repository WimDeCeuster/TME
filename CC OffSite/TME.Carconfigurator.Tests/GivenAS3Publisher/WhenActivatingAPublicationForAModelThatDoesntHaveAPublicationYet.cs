using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
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
    public class WhenActivatingAPublicationForAModelThatDoesntHaveAPublicationYet : ActivatePublicationTestBase
    {
        
        protected override void Arrange()
        {
            base.Arrange();
            var models1 = new Models();
            var models2 = new Models();
            A.CallTo(() => Service.GetModelsOverview(Brand, Country, Language1)).Returns(models1);
            A.CallTo(() => Service.GetModelsOverview(Brand, Country, Language2)).Returns(models2);
            
        }

        [Fact]
        public void ThenItShouldUploadCorrectDataForLanguage1()
        {
            A.CallTo(() => Service.PutModelsOverview(Brand, Country, Language1, null))
                .WhenArgumentsMatch(args =>
                {
                    var language = (String)args[2];
                    var models = ((Models)args[3]);
                    return ShouldContainAModelWithActivatedPublicationForLanguage(language,models,Language1,ModelNameForLanguage1);
                })
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldUploadCorrectDataForLanguage2()
        {
            A.CallTo(() => Service.PutModelsOverview(Brand, Country, Language2, null))
                .WhenArgumentsMatch(args =>
                {
                    var language = (String) args[2];
                    var models = ((Models)args[3]);
                    return ShouldContainAModelWithActivatedPublicationForLanguage(language, models, Language2, ModelNameForLanguage2);
                })
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private bool ShouldContainAModelWithActivatedPublicationForLanguage(string language, IEnumerable<Model> models, string expectedLanguage, string modelNameForLanguage)
        {
            return ShouldContainModelWithActivatedPublication(models, modelNameForLanguage) && language.Equals(expectedLanguage);
        }

        private bool ShouldContainModelWithActivatedPublication(IEnumerable<Model> models, string modelName)
        {
            var model = models.SingleOrDefault(m => m.Name.Equals(modelName));
            return model != null && model.Publications.Count == 1 && model.Publications.All(p => p.State == PublicationState.Activated);
        }
    }
}