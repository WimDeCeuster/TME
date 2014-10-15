using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.DI;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Enums;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAModel
{
    public class WhenAccessingItsBodyTypesForTheFirstTime : TestBase
    {
        private Repository.Objects.BodyType _bodyType1;
        private Repository.Objects.BodyType _bodyType2;
        private IEnumerable<IBodyType> _bodyTypes;
        private IModel _model;
        private IBodyTypeService _bodyTypeService;

        protected override void Arrange()
        {
            _bodyType1 = new BodyTypeBuilder()
                .WithId(Guid.NewGuid())
                .Build();
            _bodyType2 = new BodyTypeBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var generation = new GenerationBuilder().Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .WithGeneration(generation)
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var publicationInfo = new PublicationInfo(publication) { State = PublicationState.Activated };

            var repoModel = new ModelBuilder().AddPublication(publicationInfo).Build();

            var context = ContextBuilder.Initialize().Build();

            var publicationService = A.Fake<IPublicationService>();
            A.CallTo(() => publicationService.GetPublication(publication.ID, context)).Returns(publication);

            var modelService = A.Fake<IModelService>();
            A.CallTo(() => modelService.GetModels(A<Context>._)).Returns(new List<Repository.Objects.Model> { repoModel });

            _bodyTypeService = A.Fake<IBodyTypeService>(opt => opt.Strict());
            A.CallTo(() => _bodyTypeService.GetBodyTypes(publication.ID, publicationTimeFrame.ID, context))
                .Returns(new List<Repository.Objects.BodyType> { _bodyType1, _bodyType2 });

            var serviceFacade = new S3ServiceFacade()
                .WithConfigurationManager(new ConfigurationManagerBuilder().Build())
                .WithModelService(modelService)
                .WithPublicationService(publicationService)
                .WithBodyTypeService(_bodyTypeService);

            var modelFactory = new ModelFactoryFacade()
                .WithServiceFacade(serviceFacade)
                .Create();

            _model = modelFactory.GetModels(context).Single();
        }

        protected override void Act()
        {
            _bodyTypes = _model.BodyTypes;
        }

        [Fact]
        public void ThenItShouldFetchTheBodyTypesFromTheService()
        {
            A.CallTo(() => _bodyTypeService.GetBodyTypes(A<Guid>._, A<Guid>._, A<Context>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenListOfBodyTypesShouldBeCorrect()
        {
            _bodyTypes.Should().HaveCount(2);

            _bodyTypes.Should().Contain(bt => bt.ID == _bodyType1.ID);
            _bodyTypes.Should().Contain(bt => bt.ID == _bodyType2.ID);
        }
    }
}