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

namespace TME.CarConfigurator.Query.Tests.Services.GivenABodyTypeService
{
    public class WhenGetBodyTypesIsCalled : TestBase
    {
        private Context _context;
        private IBodyTypeService _bodyTypeService;
        private IEnumerable<Repository.Objects.BodyType> _expectedBodyTypes;
        private IEnumerable<Repository.Objects.BodyType> _actualBodyTypes;

        protected override void Arrange()
        {
            _context = new ContextBuilder().Build();

            const string s3Key = "fake s3 key";
            const string serializedObject = "this object is serialized";

            _expectedBodyTypes = new List<Repository.Objects.BodyType>
            {
                new BodyTypeBuilder().Build(),
                new BodyTypeBuilder().Build(),
            };

            var serialiser = A.Fake<ISerialiser>();
            var service = A.Fake<IService>();
            var keyManager = A.Fake<IKeyManager>();

            A.CallTo(() => keyManager.GetBodyTypesKey(A<Guid>._, A<Guid>._)).Returns(s3Key);
            A.CallTo(() => service.GetObject(_context.Brand, _context.Country, s3Key)).Returns(serializedObject);
            A.CallTo(() => serialiser.Deserialise<IEnumerable<Repository.Objects.BodyType>>(serializedObject)).Returns(_expectedBodyTypes);

            var serviceFacade = new S3ServiceFacade()
                .WithService(service)
                .WithSerializer(serialiser)
                .WithKeyManager(keyManager);

            _bodyTypeService = serviceFacade.CreateBodyTypeService();
        }

        protected override void Act()
        {
            _actualBodyTypes = _bodyTypeService.GetBodyTypes(Guid.NewGuid(), Guid.NewGuid(), _context);
        }

        [Fact]
        public void ThenItShouldReturnTheCorrectListOfBodyTypes()
        {
            _actualBodyTypes.Should().BeSameAs(_expectedBodyTypes);
        }
    }
}