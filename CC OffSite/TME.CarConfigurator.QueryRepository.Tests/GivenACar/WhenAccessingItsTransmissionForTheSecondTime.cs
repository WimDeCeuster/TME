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
    public class WhenAccessingItsTransmissionForTheSecondTime : TestBase
    {
        ICar _car;
        private Repository.Objects.Transmission _repoTransmission;
        private ITransmission _secondTransmission;
        private ITransmission _firstTransmission;

        protected override void Arrange()
        {
            _repoTransmission = new TransmissionBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoCar = new CarBuilder()
                .WithTransmission(_repoTransmission)
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

            var transmissionFactory = new TransmissionFactoryBuilder().Build();

            var carFactory = new CarFactoryBuilder()
                .WithCarService(carService)
                .WithTransmissionFactory(transmissionFactory)
                .Build();

            _car = carFactory.GetCars(publication, context).Single();

            _firstTransmission = _car.Transmission;
        }

        protected override void Act()
        {
            _secondTransmission = _car.Transmission;
        }

        [Fact]
        public void ThenTheSecondTransmissionShouldBeTheSameAsTheFirst()
        {
            _secondTransmission.Should().BeSameAs(_firstTransmission);
        }

        [Fact]
        public void ThenTheTransmissionShouldBeCorrect()
        {
            _secondTransmission.ID.Should().Be(_repoTransmission.ID);
        }
    }
}