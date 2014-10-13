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
            A.CallTo(() => Serialiser.Serialise((Publication)null))
                .WhenArgumentsMatch(args =>
                    args[0] is Publication)
                .ReturnsLazily(args => SerialisedData);
        }

        [Fact]
        public void ThenAPublicationShouldBePublished()
        {
            A.CallTo(() => PublicationPublisher.PublishPublications(Context))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenAPublishGenerationBodyTypesShouldHappen()
        {
            A.CallTo(() => BodyTypePublisher.PublishGenerationBodyTypes(Context))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenAPublishGenerationEnginesShouldHappen()
        {
            A.CallTo(() => EnginePublisher.PublishGenerationEngines(Context))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
