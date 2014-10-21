using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.DI;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAModel
{
    public class WhenAccessingItsWheelDrivesForTheSecondTime : TestBase
    {
        private Repository.Objects.WheelDrive _wheelDrive1;
        private Repository.Objects.WheelDrive _wheelDrive2;
        private IEnumerable<IWheelDrive> _secondWheelDrives;
        private IModel _model;
        private IWheelDriveService _wheelDriveService;
        private IEnumerable<IWheelDrive> _firstWheelDrives;

        protected override void Arrange()
        {
            _wheelDrive1 = new WheelDriveBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _wheelDrive2 = new WheelDriveBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var generation = new GenerationBuilder().Build();

            var timeFrame = new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .WithGeneration(generation)
                .AddTimeFrame(timeFrame)
                .Build();

            var publicationInfo = new PublicationInfo(publication) { State = Repository.Objects.Enums.PublicationState.Activated };

            var repoModel = new ModelBuilder().AddPublication(publicationInfo).Build();

            var context = ContextBuilder.Initialize().Build();

            var publicationService = A.Fake<IPublicationService>();
            A.CallTo(() => publicationService.GetPublication(publication.ID, context)).Returns(publication);

            var modelService = A.Fake<IModelService>();
            A.CallTo(() => modelService.GetModels(A<Context>._)).Returns(new[] { repoModel });

            _wheelDriveService = A.Fake<IWheelDriveService>();
            A.CallTo(() => _wheelDriveService.GetWheelDrives(publication.ID, timeFrame.ID, context)).Returns(new[] { _wheelDrive1, _wheelDrive2 });

            var serviceFacade = new S3ServiceFacade()
                .WithConfigurationManager(new ConfigurationManagerBuilder().Build())
                .WithModelService(modelService)
                .WithPublicationService(publicationService)
                .WithWheelDriveService(_wheelDriveService);

            var modelFactory = new ModelFactoryFacade()
                .WithServiceFacade(serviceFacade)
                .Create();

            _model = modelFactory.GetModels(context).Single();

            _firstWheelDrives = _model.WheelDrives;
        }

        protected override void Act()
        {
            _secondWheelDrives = _model.WheelDrives;
        }

        [Fact]
        public void ThenItShouldFetchNotTheWheelDrivesFromTheServiceAgain()
        {
            A.CallTo(() => _wheelDriveService.GetWheelDrives(A<Guid>._, A<Guid>._, A<Context>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenTheListOfWheelDrivesShouldBeCorrect()
        {
            _secondWheelDrives.Should().BeSameAs(_firstWheelDrives);
        }
    }
}
