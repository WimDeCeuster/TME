using System;
using System.Linq;
using FakeItEasy;
using TME.CarConfigurator.Repository.Objects;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAS3Publisher
{
    public class WhenPublishingAModelGeneration : Base.PublicationTestBase
    {
        private const string SerialisedData = "aaa";

        protected override void Arrange()
        {
            base.Arrange();
            A.CallTo(() => Serialiser.Serialise(null))
                .WhenArgumentsMatch(args =>
                    args[0] is Publication)
                .ReturnsLazily(args => SerialisedData);
        }

        [Fact]
        public void APublicationShouldBePublishedForEveryLanguage()
        {
            foreach (var language in Languages)
            {
                var mainLanguage = language;
                A.CallTo(() => Service.PutPublication(null,null))
                                      .WhenArgumentsMatch(args => args[0].Equals(mainLanguage) && args[1] != null)
                                      .MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        private bool TestFunction(ArgumentCollection args, string language)
        {
            var key = (String)args[0];
            var json = args[1];
            return key.Contains(language) && json.Equals(SerialisedData);
        }

        [Fact]
        public void ModelGenerationAssetsShouldBePublishedForEveryLanguage()
        {
            foreach (var language in Languages)
            {
                var mainLanguage = language;
                A.CallTo(() => Service.PutPublication(null,null))
                    .WhenArgumentsMatch(args => args[0].Equals(mainLanguage) 
                                            && ((Publication) args[1]).Generation.Assets.Count != 0)
                    .MustHaveHappened(Repeated.Exactly.Once);
            }
        }
    }
}
