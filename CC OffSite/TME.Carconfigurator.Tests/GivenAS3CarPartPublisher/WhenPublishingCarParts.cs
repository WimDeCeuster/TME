using System;
using System.Collections.Generic;
using FakeItEasy;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.CommandServices;
using TME.CarConfigurator.S3.Publisher;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAS3CarPartPublisher
{
    public class WhenPublishingCarParts : TestBase
    {
        private ICarPartPublisher _publisher;
        private IContext _context;
        private IService _s3Service;
        const string SERIALISED_CARPARTS = "serialised carparts";
        const string CAR_PART_KEY_FOR_CAR_1 = "car part key for car 1";
        const string CAR_PART_KEY_FOR_CAR_2 = "car part key for car 2";
        const string LANGUAGE1 = "de";

        protected override void Arrange()
        {
            var publication = new PublicationBuilder().WithID(Guid.NewGuid()).Build();

            var car1ID = Guid.NewGuid();
            var car2ID = Guid.NewGuid();

            var carParts = new [] {new CarPartBuilder().WithName("name1").WithCode("code1").Build(),
                                   new CarPartBuilder().WithName("name2").WithCode("code2").Build(),
                                   new CarPartBuilder().WithName("name3").WithCode("code3").Build(),
                                   new CarPartBuilder().WithName("name4").WithCode("code4").Build()};

            _context = new ContextBuilder()
                .WithLanguages(LANGUAGE1)
                .WithPublication(LANGUAGE1, publication)
                .WithCarParts(LANGUAGE1, car1ID, carParts)
                .WithCarParts(LANGUAGE1,car2ID,carParts)
                .Build();

            var keyManager = A.Fake<IKeyManager>();
            A.CallTo(() => keyManager.GetCarPartsKey(publication.ID, car1ID))
                .Returns(CAR_PART_KEY_FOR_CAR_1);
            A.CallTo(() => keyManager.GetCarPartsKey(publication.ID, car2ID))
                .Returns(CAR_PART_KEY_FOR_CAR_2);

            var serialiser = A.Fake<ISerialiser>();
            A.CallTo(() => serialiser.Serialise(A<IEnumerable<CarPart>>.That.IsSameSequenceAs(carParts)))
                .Returns(SERIALISED_CARPARTS);

            _s3Service = A.Fake<IService>();

            var carPartService = new CarPartService(_s3Service, serialiser, keyManager);
            _publisher = new CarPartPublisher(carPartService);
        }

        protected override void Act()
        {
            _publisher.PublishCarPartsAsync(_context).Wait();
        }

        [Fact]
        public void ThenCarPartsShouldBePutForAllCars()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<string>._, A<string>._, A<string>._, A<string>._))
                .MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Fact]
        public void ThenCarPartsShouldBePutForCar1()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<String>._,A<String>._,CAR_PART_KEY_FOR_CAR_1,SERIALISED_CARPARTS)).MustHaveHappened(Repeated.Exactly.Once);
        }
        
        [Fact]
        public void ThenCarPartsShouldBePutForCar2()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<String>._,A<String>._,CAR_PART_KEY_FOR_CAR_2,SERIALISED_CARPARTS)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}