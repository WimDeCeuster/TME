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
        protected override void Arrange()
        {
            base.Arrange();
            A.CallTo(() => Serialiser.Serialise(null))
                .WhenArgumentsMatch(args =>
                    args[0] is Publication)
                .ReturnsLazily(args => JsonConvert.SerializeObject(args.Arguments[0]));
        }

        [Fact]
        public void APublicationShouldBePublishedForEveryLanguage()
        {
            foreach (var language in Languages)
            { 
                var publicationKey = Service.Content.Keys.Single(key => Regex.Match(key, "nl/generation/" + GuidRegexPattern, RegexOptions.IgnoreCase).Success);           
                var publicationJson = Service.Content[publicationKey];

                var publication = JsonConvert.DeserializeObject<Publication>(publicationJson);

                publication.Should().NotBeNull();
            }
        }

        [Fact]
        public void ModelGenerationAssetsShouldBePublished()
        {

        }
    }
}
