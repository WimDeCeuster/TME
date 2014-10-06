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

            A.CallTo(() => Service.GetModelsOverviewPerLanguage(Brand, Country)).Returns(languages);
            
        }

        [Fact]
        public void ThenItShouldUploadCorrectDataForLanguage1()
        {
            A.CallTo(() => Service.PutModelsOverviewPerLanguage(Brand, Country, null))
                .WhenArgumentsMatch(args =>
                {
                    var models = ((Languages)args[2]).Single(l => l.Code.Equals(Language1)).Models;
                    return ShouldContainModelWithActivatedPublication(models, ModelNameForLanguage1);
                })
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldUploadCorrectDataForLanguage2()
        {
            A.CallTo(() => Service.PutModelsOverviewPerLanguage(Brand, Country, null))
                .WhenArgumentsMatch(args =>
                {
                    var models = ((Languages)args[2]).Single(l => l.Code.Equals(Language2)).Models;
                    return ShouldContainModelWithActivatedPublication(models, ModelNameForLanguage2);
                })
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenTheModelOverviewFileShouldOnlyBeUploadedOnce()
        {
            A.CallTo(() => Service.PutModelsOverviewPerLanguage(null,null,null))
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