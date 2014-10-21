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
    public class WhenAccessingItsTransmissionsForTheSecondTime : TestBase
    {
        private Repository.Objects.Transmission _transmission1;
        private Repository.Objects.Transmission _transmission2;
        private IEnumerable<ITransmission> _secondTransmissions;
        private IModel _model;
        private ITransmissionService _transmissionService;
        private IEnumerable<ITransmission> _firstTransmissions;

        protected override void Arrange()
        {
            _transmission1 = new TransmissionBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _transmission2 = new TransmissionBuilder()
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

            _transmissionService = A.Fake<ITransmissionService>();
            A.CallTo(() => _transmissionService.GetTransmissions(publication.ID, timeFrame.ID, context)).Returns(new[] { _transmission1, _transmission2 });

            var serviceFacade = new S3ServiceFacade()
                .WithConfigurationManager(new ConfigurationManagerBuilder().Build())
                .WithModelService(modelService)
                .WithPublicationService(publicationService)
                .WithTransmissionService(_transmissionService);

            var modelFactory = new ModelFactoryFacade()
                .WithServiceFacade(serviceFacade)
                .Create();

            _model = modelFactory.GetModels(context).Single();

            _firstTransmissions = _model.Transmissions;
        }

        protected override void Act()
        {
            _secondTransmissions = _model.Transmissions;
        }

        [Fact]
        public void ThenItShouldNotFetchTheTransmissionsFromTheServiceAgain()
        {
            A.CallTo(() => _transmissionService.GetTransmissions(A<Guid>._, A<Guid>._, A<Context>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenTheListOfTransmissionsShouldBeCorrect()
        {
            _secondTransmissions.Should().BeSameAs(_firstTransmissions);
        }
    }
}
