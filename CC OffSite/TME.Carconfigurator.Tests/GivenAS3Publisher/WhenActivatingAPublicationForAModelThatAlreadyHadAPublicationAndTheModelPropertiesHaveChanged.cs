using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Core;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAS3Publisher
{
    public class WhenActivatingAPublicationForAModelThatAlreadyHadAPublicationAndTheModelPropertiesHaveChanged : ActivatePublicationTestBase
    {
        private const string OldModelNameForLanguage1 = "tom";
        private const string OldModelNameForLanguage2 = "abe";
        private const string OldInternalCodeForLanguage1 = "OldInternalCode";
        private const string OldLocalCodeForLanguage1 = "OldLocalCode";
        private const string OldDescriptionForLanguage1 = "OldDescription";
        private const string OldFootNoteForLanguage1 = "OldFootNote";
        private const string OldTooltipForLanguage1 = "OldToolTip";
        private const int OldSortIndexForLanguage1 = 2;
        private readonly List<Label> _oldLabelsForLanguage1 = new List<Label>()
        {
            new Label(){Code = "old label 1",Value = "old value1"},
            new Label(){Code = "old label 2",Value = "old value2"},
            new Label(){Code = "old label 2",Value = "old value2"}
        };

        protected override void Arrange()
        {
            base.Arrange();
            var models1 = GetModel(OldModelNameForLanguage1,OldInternalCodeForLanguage1,OldLocalCodeForLanguage1,OldDescriptionForLanguage1,OldFootNoteForLanguage1,OldTooltipForLanguage1,OldSortIndexForLanguage1,_oldLabelsForLanguage1);
            var models2 = GetModel(OldModelNameForLanguage2, null, null, null, null, null, 0,null);
            var languages = new Languages()
            {
                new Language(Language1){Models = new List<Model>{models1}},
                new Language(Language2){Models = new List<Model>{models2}}
            };

            A.CallTo(() => LanguageService.GetModelsOverviewPerLanguage(Context)).Returns(languages);
        }

        [Fact]
        public void ThenItShouldPublishTheCorrectNewModelNameForLanguage1()
        {
            A.CallTo(() => LanguageService.PutModelsOverviewPerLanguage(null, null))
                .WhenArgumentsMatch(args =>
                {
                    var isContext = args[0].Equals(Context);
                    var model = ((Languages) args[1]).Single(l => l.Code.Equals(Language1)).Models[0];
                    return isContext && model.Name.Equals(ModelNameForLanguage1);
                })
                .MustHaveHappened(Repeated.Exactly.Once);
        }
        
        [Fact]
        public void ThenItShouldPublishTheCorrectNewModelNameForLanguage2()
        {
            A.CallTo(() => LanguageService.PutModelsOverviewPerLanguage(null, null))
                .WhenArgumentsMatch(args =>
                {
                    var isContext = args[0].Equals(Context);
                    var model = ((Languages) args[1]).Single(l => l.Code.Equals(Language2)).Models[0];
                    return isContext && model.Name.Equals(ModelNameForLanguage2);
                })
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishTheCorrectInternalCodeForLanguage1()
        {
            A.CallTo(() => LanguageService.PutModelsOverviewPerLanguage(null, null))
                .WhenArgumentsMatch(args =>
                {
                    var isContext = args[0].Equals(Context);
                    var model = ((Languages) args[1]).Single(l => l.Code.Equals(Language1)).Models[0];
                    return isContext && model.InternalCode.Equals(InternalCodeForLanguage1);
                }).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishTheCorrectLocalCodeForLanguage1()
        {
            A.CallTo(() => LanguageService.PutModelsOverviewPerLanguage(null, null)).WhenArgumentsMatch(args =>
            {
                var isContext = args[0].Equals(Context);
                var model = ((Languages)args[1]).Single(l => l.Code.Equals(Language1)).Models[0];
                return isContext && model.LocalCode.Equals(LocalCodeForLanguage1);
            }).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishTheCorrectDescriptionForLanguage1()
        {
            A.CallTo(() => LanguageService.PutModelsOverviewPerLanguage(null, null)).WhenArgumentsMatch(args =>
            {
                var isContext = args[0].Equals(Context);
                var model = ((Languages)args[1]).Single(l => l.Code.Equals(Language1)).Models[0];
                return isContext && model.Description.Equals(DescriptionForLanguage1);
            }).MustHaveHappened(Repeated.Exactly.Once);
        }
        
        [Fact]
        public void ThenItShouldPublishTheCorrectFootNoteForLanguage1()
        {
            A.CallTo(() => LanguageService.PutModelsOverviewPerLanguage(null, null)).WhenArgumentsMatch(args =>
            {
                var isContext = args[0].Equals(Context);
                var model = ((Languages)args[1]).Single(l => l.Code.Equals(Language1)).Models[0];
                return isContext && model.FootNote.Equals(FootNoteForLanguage1);
            }).MustHaveHappened(Repeated.Exactly.Once);
        }
        
        [Fact]
        public void ThenItShouldPublishTheCorrectTooltipForLanguage1()
        {
            A.CallTo(() => LanguageService.PutModelsOverviewPerLanguage(null, null)).WhenArgumentsMatch(args =>
            {
                var isContext = args[0].Equals(Context);
                var model = ((Languages)args[1]).Single(l => l.Code.Equals(Language1)).Models[0];
                return isContext && model.ToolTip.Equals(TooltipForLanguage1);
            }).MustHaveHappened(Repeated.Exactly.Once);
        }
        
        [Fact]
        public void ThenItShouldPublishTheCorrectSortIndexForLanguage1()
        {
            A.CallTo(() => LanguageService.PutModelsOverviewPerLanguage(null, null)).WhenArgumentsMatch(args =>
            {
                var isContext = args[0].Equals(Context);
                var model = ((Languages)args[1]).Single(l => l.Code.Equals(Language1)).Models[0];
                return model.SortIndex.Equals(SortIndexForLanguage1);
            }).MustHaveHappened(Repeated.Exactly.Once);
        }
        
        [Fact]
        public void ThenItShouldPublishTheCorrectLabelsForLanguage1()
        {
            A.CallTo(() => LanguageService.PutModelsOverviewPerLanguage(null, null)).WhenArgumentsMatch(args =>
            {
                var isContext = args[0].Equals(Context);
                var model = ((Languages)args[1]).Single(l => l.Code.Equals(Language1)).Models[0];
                return model.Labels.Equals(LabelsForLanguage1);
            }).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}