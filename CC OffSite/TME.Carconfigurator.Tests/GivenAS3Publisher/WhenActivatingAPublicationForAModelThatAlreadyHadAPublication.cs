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

namespace TME.Carconfigurator.Tests.GivenAS3Publisher
{
    public class WhenActivatingAPublicationForAModelThatAlreadyHadAPublication : ActivatePublicationTestBase
    {
        protected override void Arrange()
        {
            var models1 = GetModel(ModelNameForLanguage1);
            var models2 = GetModel(ModelNameForLanguage2);

            A.CallTo(() => Service.GetModelsOverview(Brand, Country, Language1)).Returns(models1);
            A.CallTo(() => Service.GetModelsOverview(Brand, Country, Language2)).Returns(models2);
            A.CallTo(() => Service.PutModelsOverview(null, null, null, null)).WithAnyArguments();
            A.CallTo(() => Service.PutObject(null, null)).WithAnyArguments();
        }

        private Models GetModel(string modelName)
        {
            return new Models()
            {
                new Model {Name = modelName,ID = ModelID, Publications =
                {
                    new PublicationInfo(new Publication{ID = Guid.NewGuid(),Generation = new Generation()})
                }}
            };
        }

        protected override void Act()
        {
            Publisher.Publish(Context);
        }

        [Fact]
        public void ThenItShouldUploadCorrectDataForLanguage1()
        {
            A.CallTo(() => Service.PutModelsOverview(Brand, Country, Language1, null))
                .WhenArgumentsMatch(args =>
                {
                    var language = (String)args[2];
                    var models = ((Models)args[3]);
                    return ShouldContainAModelWithActivatedPublicationAndDeletedPublicationForLanguage(language, models, Language1, ModelNameForLanguage1);
                })
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private bool ShouldContainAModelWithActivatedPublicationAndDeletedPublicationForLanguage(string language, IEnumerable<Model> models, string expectedLanguage, string modelNameForLanguage)
        {
            return ShouldContainModelWithActivatedPublicationAndDeletedPublication(models, modelNameForLanguage) && language.Equals(expectedLanguage);
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