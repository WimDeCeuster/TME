﻿using System.Linq;
using FakeItEasy;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Repository.Objects;
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
            var languages = new Languages()
            {
                new Language(Language1){Models = new Repository<Model>{models1}},
                new Language(Language2){Models = new Repository<Model>{models2}}
            };

            A.CallTo(() => Service.GetModelsOverview(Brand, Country)).Returns(languages);
        }

        [Fact]
        public void ThenItShouldPublishTheCorrectNewModelNameForLanguage1()
        {
            A.CallTo(() => Service.PutModelsOverview(Brand, Country, null))
                .WhenArgumentsMatch(args =>
                {
                    var model = ((Languages) args[2]).Single(l => l.Code.Equals(Language1)).Models[0];
                    return model.Name.Equals(ModelNameForLanguage1);
                })
                .MustHaveHappened(Repeated.Exactly.Once);
        }
        
        [Fact]
        public void ThenItShouldPublishTheCorrectNewModelNameForLanguage2()
        {
            A.CallTo(() => Service.PutModelsOverview(Brand, Country, null))
                .WhenArgumentsMatch(args =>
                {
                    var model = ((Languages) args[2]).Single(l => l.Code.Equals(Language2)).Models[0];
                    return model.Name.Equals(ModelNameForLanguage2);
                })
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}