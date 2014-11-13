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

namespace TME.CarConfigurator.Query.Tests.Services.GivenACarPartService
{
    public class WhenGetCarPartsIsCalled : TestBase
    {
        private Context _context;
        private IEnumerable<Repository.Objects.CarPart> _expectedCarParts;
        private IEnumerable<Repository.Objects.CarPart> _actualCarParts;
        private ICarPartService _carPartService;

        protected override void Arrange()
        {
            _context = ContextBuilder.Initialize().Build();

            const string s3Key = "fake s3 key";
            const string serializedObject = "this object is serialized";

            _expectedCarParts = new List<Repository.Objects.CarPart>
            {
                new CarPartBuilder().Build(),
                new CarPartBuilder().Build(),
            };

            var serialiser = A.Fake<ISerialiser>();
            var service = A.Fake<IService>();
            var keyManager = A.Fake<IKeyManager>();

            A.CallTo(() => keyManager.GetCarPartsKey(A<Guid>._, A<Guid>._)).Returns(s3Key);
            A.CallTo(() => service.GetObject(_context.Brand, _context.Country, s3Key)).Returns(serializedObject);
            A.CallTo(() => serialiser.Deserialise<IEnumerable<Repository.Objects.CarPart>>(serializedObject)).Returns(_expectedCarParts);

            var serviceFacade = new S3ServiceFacade()
                .WithService(service)
                .WithSerializer(serialiser)
                .WithKeyManager(keyManager);

            _carPartService = serviceFacade.CreateCarPartService();
        }

        protected override void Act()
        {
            _actualCarParts = _carPartService.GetCarParts(Guid.NewGuid(), Guid.NewGuid(), _context);
        }

        [Fact]
        public void ThenItShouldReturnTheCorrectListOfCarParts()
        {
            _actualCarParts.Should().BeSameAs(_expectedCarParts);
        }
    }
}