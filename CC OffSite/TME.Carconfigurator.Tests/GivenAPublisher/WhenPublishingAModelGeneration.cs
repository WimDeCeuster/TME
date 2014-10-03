using FakeItEasy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Repository.Objects;
using Xunit;
using FluentAssertions;

namespace TME.Carconfigurator.Tests.GivenAPublisher
{
    public class WhenPublishingAModelGeneration : Base.PublicationTest
    {
        protected override void Arrange()
        {
            BaseArrange();
            A.CallTo(() => Serialiser.Serialise(null))
                .WhenArgumentsMatch(args =>
                    args[0] is Publication)
                .ReturnsLazily(args => JsonConvert.SerializeObject(args.Arguments[0]));
        }

        protected override void Act()
        {
            BaseAct();
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
