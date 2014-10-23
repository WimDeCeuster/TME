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
            A.CallTo(() => Serialiser.Serialise(A<Publication>._)).ReturnsLazily(args => SerialisedData);
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

        [Fact]
        public void ThenAPublishGenerationTransmissionsShouldHappen()
        {
            A.CallTo(() => TransmissionPublisher.PublishGenerationTransmissions(Context))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenAPublishGenerationWheelDrivesShouldHappen()
        {
            A.CallTo(() => WheelDrivePublisher.PublishGenerationWheelDrives(Context))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenAPublishGenerationSteeringsShouldHappen()
        {
            A.CallTo(() => SteeringPublisher.PublishGenerationSteerings(Context))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenAPublishGenerationGradesShouldHappen()
        {
            A.CallTo(() => GradePublisher.PublishGenerationGrades(Context))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenAPublishGenerationCarsShouldHappen()
        {
            A.CallTo(() => CarPublisher.PublishGenerationCars(Context))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishGenerationAssets()
        {
            A.CallTo(() => AssetPublisher.PublishAssets(Context))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishCarAssets()
        {
            A.CallTo(() => AssetPublisher.PublishCarAssets(Context))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishGenerationSubModels()
        {
            A.CallTo(() => SubModelPublisher.PublishGenerationSubModelsAsync(Context))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishGenerationGradeAccessories()
        {
            A.CallTo(() => GradeAccessoryPublisher.Publish(Context))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldPublishGenerationGradeOptions()
        {
            A.CallTo(() => GradeOptionPublisher.Publish(Context))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
