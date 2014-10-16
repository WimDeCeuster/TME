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

namespace TME.CarConfigurator.Query.Tests.Services.GivenATransmissionService
{
    public class WhenGetTransmissionsIsCalled : TestBase
    {
        private Context _context;
        private ITransmissionService _transmissionService;
        private IEnumerable<Repository.Objects.Transmission> _expectedTransmissions;
        private IEnumerable<Repository.Objects.Transmission> _actualTransmissions;

        protected override void Arrange()
        {
            _context = ContextBuilder.Initialize().Build();

            const string s3Key = "fake s3 key";
            const string serializedObject = "this object is serialized";

            _expectedTransmissions = new List<Repository.Objects.Transmission>
            {
                new TransmissionBuilder().Build(),
                new TransmissionBuilder().Build(),
            };

            var serialiser = A.Fake<ISerialiser>();
            var service = A.Fake<IService>();
            var keyManager = A.Fake<IKeyManager>();

            A.CallTo(() => keyManager.GetTransmissionsKey(A<Guid>._, A<Guid>._)).Returns(s3Key);
            A.CallTo(() => service.GetObject(_context.Brand, _context.Country, s3Key)).Returns(serializedObject);
            A.CallTo(() => serialiser.Deserialise<IEnumerable<Repository.Objects.Transmission>>(serializedObject)).Returns(_expectedTransmissions);

            var serviceFacade = new S3ServiceFacade()
                .WithService(service)
                .WithSerializer(serialiser)
                .WithKeyManager(keyManager);

            _transmissionService = serviceFacade.CreateTransmissionService();
        }

        protected override void Act()
        {
            _actualTransmissions = _transmissionService.GetTransmissions(Guid.NewGuid(), Guid.NewGuid(), _context);
        }

        [Fact]
        public void ThenItShouldReturnTheCorrectListOfTransmissions()
        {
            _actualTransmissions.Should().BeSameAs(_expectedTransmissions);
        }
    }
}