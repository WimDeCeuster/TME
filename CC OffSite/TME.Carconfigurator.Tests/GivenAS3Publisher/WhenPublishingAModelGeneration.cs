using System;
using System.Linq;
using System.Text.RegularExpressions;
using FakeItEasy;
using FluentAssertions;
using Newtonsoft.Json;
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
                                      .WhenArgumentsMatch(args => args[0].Equals(language) && args[1] != null)
                                      .MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public void ModelGenerationAssetsShouldBePublished()
        {

        }
    }
}
