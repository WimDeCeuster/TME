using System;
using System.Collections.Generic;
using FakeItEasy;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.TechnicalSpecifications;
using TME.CarConfigurator.S3.CommandServices;
using TME.CarConfigurator.S3.Publisher;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenASpecificationPublisher
{
    public class WhenPublishingCarSpecifications : TestBase
    {
        IService _s3Service;
        ISpecificationsService _service;
        ISpecificationsPublisher _publisher;
        IContext _context;
        const string SERIALISED_CARSPECS = "serialised car specs";
        const string CAR_SPEC_KEY_FOR_CAR_1 = "car spec key for car 1";
        const string CAR_SPEC_KEY_FOR_CAR_2 = "car spec key for car 2";
        const string LANGUAGE1 = "de";

        protected override void Arrange()
        {
            var publication = new PublicationBuilder().WithID(Guid.NewGuid()).Build();

            var car1ID = Guid.NewGuid();
            var car2ID = Guid.NewGuid();

            var carSpecs = new [] {new CarSpecBuilder().WithId(new Guid()).WithName("name1").Build(),
                                   new CarSpecBuilder().WithId(new Guid()).WithName("name2").Build(),
                                   new CarSpecBuilder().WithId(new Guid()).WithName("name3").Build(),
                                   new CarSpecBuilder().WithId(new Guid()).WithName("name4").Build()};

            _context = new ContextBuilder()
                .WithLanguages(LANGUAGE1)
                .WithPublication(LANGUAGE1, publication)
                .WithCarSpecs(LANGUAGE1, car1ID, carSpecs)
                .WithCarSpecs(LANGUAGE1, car2ID, carSpecs)
                .Build();

            var keyManager = A.Fake<IKeyManager>();
            A.CallTo(() => keyManager.GetCarTechnicalSpecificationsKey(publication.ID, car1ID))
                .Returns(CAR_SPEC_KEY_FOR_CAR_1);
            A.CallTo(() => keyManager.GetCarTechnicalSpecificationsKey(publication.ID, car2ID))
                .Returns(CAR_SPEC_KEY_FOR_CAR_2);

            var serialiser = A.Fake<ISerialiser>();
            A.CallTo(() => serialiser.Serialise(A<IEnumerable<CarTechnicalSpecification>>.That.IsSameSequenceAs(carSpecs)))
                .Returns(SERIALISED_CARSPECS);

            _s3Service = A.Fake<IService>();

            _service = new SpecificationsService(_s3Service, serialiser, keyManager);
            _publisher = new SpecificationsPublisherBuilder().WithService(_service).Build();
        }

        protected override void Act()
        {
            _publisher.PublishCarTechnicalSpecificationsAsync(_context).Wait();
        }

        [Fact]
        public void ThenCarSpecsShouldBePutForAllCars()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<string>._, A<string>._, A<string>._, A<string>._))
                .MustHaveHappened(Repeated.Exactly.Twice);
        }

        //[Fact]
        //public void ThenCarSpecsShouldBePutForCar1()
        //{
        //    A.CallTo(() => _s3Service.PutObjectAsync(A<String>._, A<String>._, CAR_SPEC_KEY_FOR_CAR_1, SERIALISED_CARSPECS))
        //        .MustHaveHappened(Repeated.Exactly.Once);
        //}
        
        //[Fact]
        //public void ThenCarSpecsShouldBePutForCar2()
        //{
        //    A.CallTo(() => _s3Service.PutObjectAsync(A<String>._, A<String>._, CAR_SPEC_KEY_FOR_CAR_2, SERIALISED_CARSPECS))
        //        .MustHaveHappened(Repeated.Exactly.Once);
        //}
    }
}