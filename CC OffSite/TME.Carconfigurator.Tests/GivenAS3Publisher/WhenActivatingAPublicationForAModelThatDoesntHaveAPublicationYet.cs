using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Enums;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAS3Publisher
{
    public class WhenActivatingAPublicationForAModelThatDoesntHaveAPublicationYet : ActivatePublicationTestBase
    {
        
        protected override void Arrange()
        {
            base.Arrange();
            var languages = new Languages();

            A.CallTo(() => GetModelService.GetModelsByLanguage(Context.Brand, Context.Country)).Returns(languages);
            
        }

        [Fact]
        public void ThenItShouldUploadCorrectDataForLanguage1()
        {
            A.CallTo(() => PutLanguageService.PutModelsOverviewPerLanguage(null, null))
                .WhenArgumentsMatch(args =>
                {
                    var isContext = args[0].Equals(Context);
                    var models = ((Languages)args[1]).Single(l => l.Code.Equals(Language1)).Models;
                    return isContext && ShouldContainModelWithActivatedPublication(models, ModelNameForLanguage1);
                })
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldUploadCorrectDataForLanguage2()
        {
            A.CallTo(() => PutLanguageService.PutModelsOverviewPerLanguage(null, null))
                .WhenArgumentsMatch(args =>
                {
                    var isContext = args[0].Equals(Context);
                    var models = ((Languages)args[1]).Single(l => l.Code.Equals(Language2)).Models;
                    return isContext && ShouldContainModelWithActivatedPublication(models, ModelNameForLanguage2);
                })
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenTheModelOverviewFileShouldOnlyBeUploadedOnce()
        {
            A.CallTo(() => PutLanguageService.PutModelsOverviewPerLanguage(null, null))
                .WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private bool ShouldContainModelWithActivatedPublication(IEnumerable<Model> models, string modelName)
        {
            var model = models.SingleOrDefault(m => m.Name.Equals(modelName));
            return model != null && model.Publications.Count == 1 && model.Publications.All(p => p.State == PublicationState.Activated);
        }
    }
}