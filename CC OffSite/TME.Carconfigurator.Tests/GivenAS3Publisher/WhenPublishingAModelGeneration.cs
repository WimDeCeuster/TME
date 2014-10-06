using System;
using System.Linq;
using System.Text.RegularExpressions;
using FakeItEasy;
using FluentAssertions;
using Newtonsoft.Json;
using TME.CarConfigurator.Interfaces;
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
                A.CallTo(() => Service.PutObject(null,null))
                                      .WhenArgumentsMatch(args => TestFunction(args, mainLanguage))
                                      .MustHaveHappened();
            }
        }

        [Fact]
        public void ModelGenerationAssetsShouldBePublished()
        {
            foreach (var language in Languages)
            {
                var mainLanguage = language;
                
//                A.CallTo(() => Service.PutObject(null,null))
//                    .WhenArgumentsMatch(args =>
//                    {
//                        TestFunction(args, mainLanguage);
//                        IModel model = ((Languages) args[1]).Single(l => l.Code.Equals(mainLanguage)).Models[0];
//                    });
            }
        }

        private bool TestFunction(ArgumentCollection args, string language)
        {
            var key = (String)args[0];
            var json = args[1];
            return key.Contains(language) && json.Equals(SerialisedData);
        }
    }
}
