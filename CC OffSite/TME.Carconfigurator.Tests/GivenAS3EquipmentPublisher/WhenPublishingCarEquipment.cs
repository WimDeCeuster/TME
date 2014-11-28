using System;
using FakeItEasy;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.S3.CommandServices;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAS3EquipmentPublisher
{
    public class WhenPublishingCarEquipment : TestBase
    {
        private IEquipmentPublisher _publisher;
        private IContext _context;
        private IService _s3Service;
        const string SERIALISED_CAREQUIPMENT = "serialised car equipment data";
        const string CAR_EQUIPMENT_KEY_FOR_CAR_1 = "car equipment key for car 1";
        const string CAR_EQUIPMENT_KEY_FOR_CAR_2 = "car equipment key for car 2";
        const String LANGUAGE1 = "de";

        protected override void Arrange()
        {
            var carEquipment = new CarEquipmentBuilder()
                .Build();

            var publication = new PublicationBuilder().WithID(Guid.NewGuid()).Build();

            var car1ID = Guid.NewGuid();
            var car2ID = Guid.NewGuid();

            _context = new ContextBuilder()
                .WithLanguages(LANGUAGE1)
                .WithPublication(LANGUAGE1, publication)
                .WithCarEquipment(LANGUAGE1, car1ID, carEquipment)
                .WithCarEquipment(LANGUAGE1, car2ID, carEquipment)
                .Build();

            _s3Service = A.Fake<IService>();

            var serialiser = A.Fake<ISerialiser>();
            A.CallTo(() => serialiser.Serialise(A<CarEquipment>._)).Returns(SERIALISED_CAREQUIPMENT);

            var keymanager = A.Fake<IKeyManager>();
            A.CallTo(() => keymanager.GetCarEquipmentKey(publication.ID, car1ID)).Returns(CAR_EQUIPMENT_KEY_FOR_CAR_1);
            A.CallTo(() => keymanager.GetCarEquipmentKey(publication.ID, car2ID)).Returns(CAR_EQUIPMENT_KEY_FOR_CAR_2);

            var equipmentService = new EquipmentService(_s3Service, serialiser, keymanager);
            _publisher = new EquipmentPublisherBuilder().WithService(equipmentService).Build();
        }

        protected override void Act()
        {
            _publisher.PublishCarEquipmentAsync(_context).Wait();
        }

        [Fact]
        public void ThenCarEquipmentShouldBePutForAllCars()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<String>._, A<String>._, A<String>._, A<String>._)).MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Fact]
        public void ThenCarEquipmentShouldBePutForCar1()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<String>._, A<String>._, CAR_EQUIPMENT_KEY_FOR_CAR_1, SERIALISED_CAREQUIPMENT)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenCarEquipmentShouldBePutForCar2()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<String>._, A<String>._, CAR_EQUIPMENT_KEY_FOR_CAR_2, SERIALISED_CAREQUIPMENT)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}