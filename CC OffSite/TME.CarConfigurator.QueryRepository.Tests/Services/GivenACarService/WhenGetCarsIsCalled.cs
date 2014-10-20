using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.DI;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.Services.GivenACarService
{
    public class WhenGetCarsIsCalled : TestBase
    {
        private Context _context;
        private ICarService _carService;
        private IEnumerable<Repository.Objects.Car> _expectedCars;
        private IEnumerable<Repository.Objects.Car> _actualCars;

        protected override void Arrange()
        {
            _context = ContextBuilder.Initialize().Build();

            const string s3Key = "fake s3 key";
            const string serializedObject = "this object is serialized";

            _expectedCars = new List<Repository.Objects.Car>
            {
                new CarBuilder().Build(),
                new CarBuilder().Build(),
            };

            var serialiser = A.Fake<ISerialiser>();
            var service = A.Fake<IService>();
            var keyManager = A.Fake<IKeyManager>();

            A.CallTo(() => keyManager.GetCarsKey(A<Guid>._, A<Guid>._)).Returns(s3Key);
            A.CallTo(() => service.GetObject(_context.Brand, _context.Country, s3Key)).Returns(serializedObject);
            A.CallTo(() => serialiser.Deserialise<IEnumerable<Repository.Objects.Car>>(serializedObject)).Returns(_expectedCars);

            var serviceFacade = new S3ServiceFacade()
                .WithService(service)
                .WithSerializer(serialiser)
                .WithKeyManager(keyManager);

            _carService = serviceFacade.CreateCarService();
        }

        protected override void Act()
        {
            _actualCars = _carService.GetCars(Guid.NewGuid(), Guid.NewGuid(), _context);
        }

        [Fact]
        public void ThenItShouldReturnTheCorrectListOfCars()
        {
            _actualCars.Should().BeSameAs(_expectedCars);
        }
    }
}