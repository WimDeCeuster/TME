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

namespace TME.CarConfigurator.Query.Tests.Services.GivenAWheelDriveService
{
    public class WhenGetWheelDrivesIsCalled : TestBase
    {
        private Context _context;
        private IWheelDriveService _wheelDriveService;
        private IEnumerable<Repository.Objects.WheelDrive> _expectedWheelDrives;
        private IEnumerable<Repository.Objects.WheelDrive> _actualWheelDrives;

        protected override void Arrange()
        {
            _context = ContextBuilder.Initialize().Build();

            const string s3Key = "fake s3 key";
            const string serializedObject = "this object is serialized";

            _expectedWheelDrives = new List<Repository.Objects.WheelDrive>
            {
                new WheelDriveBuilder().Build(),
                new WheelDriveBuilder().Build(),
            };

            var serialiser = A.Fake<ISerialiser>();
            var service = A.Fake<IService>();
            var keyManager = A.Fake<IKeyManager>();

            A.CallTo(() => keyManager.GetWheelDrivesKey(A<Guid>._, A<Guid>._)).Returns(s3Key);
            A.CallTo(() => service.GetObject(_context.Brand, _context.Country, s3Key)).Returns(serializedObject);
            A.CallTo(() => serialiser.Deserialise<IEnumerable<Repository.Objects.WheelDrive>>(serializedObject)).Returns(_expectedWheelDrives);

            var serviceFacade = new S3ServiceFacade()
                .WithService(service)
                .WithSerializer(serialiser)
                .WithKeyManager(keyManager);

            _wheelDriveService = serviceFacade.CreateWheelDriveService();
        }

        protected override void Act()
        {
            _actualWheelDrives = _wheelDriveService.GetWheelDrives(Guid.NewGuid(), Guid.NewGuid(), _context);
        }

        [Fact]
        public void ThenItShouldReturnTheCorrectListOfWheelDrives()
        {
            _actualWheelDrives.Should().BeSameAs(_expectedWheelDrives);
        }
    }
}