using System;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenACar
{
    public class WhenAccessingItsSubModelForTheFirstTime : TestBase
    {
        ICar _car;
        private Repository.Objects.SubModel _repoSubModel;
        private ISubModel _subModel;

        protected override void Arrange()
        {
            _repoSubModel = new SubModelBuilder()
                .WithID(Guid.NewGuid())
                .Build();

            var repoCar = new CarBuilder()
                .WithSubModel(_repoSubModel)
                .Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            var carService = A.Fake<ICarService>();
            A.CallTo(() => carService.GetCars(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new[] { repoCar });

            var subModelFactory = new SubModelFactoryBuilder().Build();

            var carFactory = new CarFactoryBuilder()
                .WithCarService(carService)
                .WithSubModelFactory(subModelFactory)
                .Build();

            _car = carFactory.GetCars(publication, context).Single();
        }

        protected override void Act()
        {
            _subModel = _car.SubModel;
        }

        [Fact]
        public void ThenTheSubModelShouldBeCorrect()
        {
            _subModel.ID.Should().Be(_repoSubModel.ID);
        }
    }
}