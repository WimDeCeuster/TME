using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Enums;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAS3Publisher
{
    public class WhenActivatingAPublicationForAModelThatAlreadyHadAPublication : ActivatePublicationTestBase
    {
        protected override void Arrange()
        {
            base.Arrange();
            var models1 = GetModel(ModelNameForLanguage1, null, null, null, null, null, 0,null);
            var models2 = GetModel(ModelNameForLanguage2, null, null, null, null, null, 0,null);
            var languages = new Languages()
            {
                new Language(Language1){Models = new Repository<Model>{models1}},
                new Language(Language2){Models = new Repository<Model>{models2}}
            };

            A.CallTo(() => LanguageService.GetModelsOverviewPerLanguage(Context)).Returns(languages);
        }

        

        [Fact]
        public void ThenItShouldUploadCorrectDataForLanguage1()
        {
            A.CallTo(() => LanguageService.PutModelsOverviewPerLanguage(null, null))
                .WhenArgumentsMatch(args =>
                {
                    var isContext = args[0].Equals(Context);
                    var models = ((Languages)args[1]).Single(l=>l.Code.Equals(Language1)).Models;
                    return isContext && ShouldContainModelWithActivatedPublicationAndDeletedPublication(models, ModelNameForLanguage1);
                })
                .MustHaveHappened(Repeated.Exactly.Once);
        }
        
        [Fact]
        public void ThenItShouldUploadCorrectDataForLanguage2()
        {
            A.CallTo(() => LanguageService.PutModelsOverviewPerLanguage(null, null))
                .WhenArgumentsMatch(args =>
                {
                    var isContext = args[0].Equals(Context);
                    var models = ((Languages)args[1]).Single(l=>l.Code.Equals(Language2)).Models;
                    return isContext &&  ShouldContainModelWithActivatedPublicationAndDeletedPublication(models, ModelNameForLanguage2);
                })
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenTheModelOverviewFileShouldOnlyBeUploadedOnce()
        {
            A.CallTo(() => LanguageService.PutModelsOverviewPerLanguage(null, null))
                .WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private bool ShouldContainModelWithActivatedPublicationAndDeletedPublication(IEnumerable<Model> models, string modelName)
        {
            var model = models.SingleOrDefault(m => m.Name.Equals(modelName));
            return model != null && model.Publications.Count == 2
                   && model.Publications.Any(p => p.State == PublicationState.Activated)
                   && model.Publications.Any(p => p.State == PublicationState.ToBeDeleted);
        }
    }
}