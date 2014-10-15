using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.DI;
using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAModel
{
    public class WhenAccessingItsCarsForTheFirstTime : TestBase
    {
        private Repository.Objects.Car _car1;
        private Repository.Objects.Car _car2;
        private IEnumerable<ICar> _cars;
        private IModel _model;
        private ICarService _carService;

        protected override void Arrange()
        {
            _car1 = new CarBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _car2 = new CarBuilder()
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

            _carService = A.Fake<ICarService>();
            A.CallTo(() => _carService.GetCars(publication.ID, timeFrame.ID, context)).Returns(new [] {_car1, _car2});

            var serviceFacade = new S3ServiceFacade()
                .WithConfigurationManager(new ConfigurationManagerBuilder().Build())
                .WithModelService(modelService)
                .WithPublicationService(publicationService)
                .WithCarService(_carService);

            var modelFactory = new ModelFactoryFacade()
                .WithServiceFacade(serviceFacade)
                .Create();

            _model = modelFactory.GetModels(context).Single();
        }

        protected override void Act()
        {
            _cars = _model.Cars;
        }

        [Fact]
        public void ThenItShouldFetchTheCarsFromTheService()
        {
            A.CallTo(() => _carService.GetCars(A<Guid>._, A<Guid>._, A<Context>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenTheListOfCarsShouldBeCorrect()
        {
            _cars.Should().HaveCount(2);

            _cars.Should().Contain(car => car.ID == _car1.ID);
            _cars.Should().Contain(car => car.ID == _car2.ID);
        }
    }
}
