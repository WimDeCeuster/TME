using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAPublisher
{
    public class WhenPublishingAPublication : Base.PublicationTest
    {
        [Fact]
        public void APublicationShouldBePublished()
        {
            var nlPublicationKey = Service.Content.Keys.Single(key => Regex.Match(key, "nl/generation/" +GuidRegexPattern, RegexOptions.IgnoreCase).Success);           
            var nlPublication = Service.Content[nlPublicationKey];

            var expected = "publicationInfo";


            Assert.Equal(expected, nlPublication);
        }
    }
}
