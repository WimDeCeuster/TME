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

namespace TME.CarConfigurator.Query.Tests.Services.GivenASteeringService
{
    public class WhenGetSteeringsIsCalled : TestBase
    {
        private Context _context;
        private ISteeringService _steeringService;
        private IEnumerable<Repository.Objects.Steering> _expectedSteerings;
        private IEnumerable<Repository.Objects.Steering> _actualSteerings;

        protected override void Arrange()
        {
            _context = ContextBuilder.Initialize().Build();

            const string s3Key = "fake s3 key";
            const string serializedObject = "this object is serialized";

            _expectedSteerings = new List<Repository.Objects.Steering>
            {
                new SteeringBuilder().Build(),
                new SteeringBuilder().Build(),
            };

            var serialiser = A.Fake<ISerialiser>();
            var service = A.Fake<IService>();
            var keyManager = A.Fake<IKeyManager>();

            A.CallTo(() => keyManager.GetSteeringsKey(A<Guid>._, A<Guid>._)).Returns(s3Key);
            A.CallTo(() => service.GetObject(_context.Brand, _context.Country, s3Key)).Returns(serializedObject);
            A.CallTo(() => serialiser.Deserialise<IEnumerable<Repository.Objects.Steering>>(serializedObject)).Returns(_expectedSteerings);

            var serviceFacade = new S3ServiceFacade()
                .WithService(service)
                .WithSerializer(serialiser)
                .WithKeyManager(keyManager);

            _steeringService = serviceFacade.CreateSteeringService();
        }

        protected override void Act()
        {
            _actualSteerings = _steeringService.GetSteerings(Guid.NewGuid(), Guid.NewGuid(), _context);
        }

        [Fact]
        public void ThenItShouldReturnTheCorrectListOfSteerings()
        {
            _actualSteerings.Should().BeSameAs(_expectedSteerings);
        }
    }
}