using System;
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

namespace TME.CarConfigurator.Query.Tests.Services.GivenAGradeEquipmentService
{
    public class WhenGetSubModelGradeEquipmentIsCalled : TestBase
    {
        private Repository.Objects.Equipment.GradeEquipment _actualGradeEquipment;
        private Repository.Objects.Equipment.GradeEquipment _expectedSubModelGradeEquipment;
        private IEquipmentService _gradeEquipmentService;
        private Context _context;

        protected override void Arrange()
        {
            _context = new ContextBuilder().Build();

            const string s3Key = "fake s3 key";
            const string serializedObject = "this object is serialized";

            _expectedSubModelGradeEquipment = new GradeEquipmentBuilder().Build();

            var serialiser = A.Fake<ISerialiser>();
            var service = A.Fake<IService>();
            var keyManager = A.Fake<IKeyManager>();

            A.CallTo(() => keyManager.GetSubModelGradeEquipmentsKey(A<Guid>._, A<Guid>._, A<Guid>._, A<Guid>._)).Returns(s3Key);
            A.CallTo(() => service.GetObject(_context.Brand, _context.Country, s3Key)).Returns(serializedObject);
            A.CallTo(() => serialiser.Deserialise<Repository.Objects.Equipment.GradeEquipment>(serializedObject)).Returns(_expectedSubModelGradeEquipment);

            var serviceFacade = new S3ServiceFacade()
                .WithService(service)
                .WithSerializer(serialiser)
                .WithKeyManager(keyManager);

            _gradeEquipmentService = serviceFacade.CreateGradeEquipmentService();
        }

        protected override void Act()
        {
            _actualGradeEquipment = _gradeEquipmentService.GetSubModelGradeEquipment(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),Guid.NewGuid(), _context);
        }

        [Fact]
        public void ThenItShouldReturnTheCorrectListOfAssets()
        {
            _actualGradeEquipment.Should().BeSameAs(_expectedSubModelGradeEquipment);
        }
    }
}