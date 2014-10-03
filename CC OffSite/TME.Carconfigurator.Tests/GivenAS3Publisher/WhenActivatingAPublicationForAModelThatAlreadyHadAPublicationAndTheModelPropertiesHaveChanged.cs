using System;
using FakeItEasy;
using TME.CarConfigurator.Repository.Objects;
using TME.Carconfigurator.Tests.Base;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAS3Publisher
{
    public class WhenActivatingAPublicationForAModelThatAlreadyHadAPublicationAndTheModelPropertiesHaveChanged : ActivatePublicationTestBase
    {
        private const string OldModelNameForLanguage1 = "tom";
        private const string OldModelNameForLanguage2 = "abe";

        protected override void Arrange()
        {
            base.Arrange();
            var models1 = GetModel(OldModelNameForLanguage1);
            var models2 = GetModel(OldModelNameForLanguage2);
            A.CallTo(() => Service.GetModelsOverview(Brand, Country, Language1)).Returns(models1);
            A.CallTo(() => Service.GetModelsOverview(Brand, Country, Language2)).Returns(models2);
        }

        [Fact]
        public void ThenItShouldPublishTheCorrectNewModelNameForLanguage1()
        {
            A.CallTo(() => Service.PutModelsOverview(Brand, Country, Language1, null))
                .WhenArgumentsMatch(args =>
                {
                    var model = ((Models)args[3])[0];
                    return model.Name.Equals(ModelNameForLanguage1);
                })
                .MustHaveHappened(Repeated.Exactly.Once);
        }
        
        [Fact]
        public void ThenItShouldPublishTheCorrectNewModelNameForLanguage2()
        {
            A.CallTo(() => Service.PutModelsOverview(Brand, Country, Language2, null))
                .WhenArgumentsMatch(args =>
                {
                    var model = ((Models)args[3])[0];
                    return model.Name.Equals(ModelNameForLanguage2);
                })
                .MustHaveHappened(Repeated.Exactly.Once);
        }

    }
}