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

namespace TME.CarConfigurator.Query.Tests.Services.GivenAnEngineService
{
    public class WhenGetEnginesIsCalled : TestBase
    {
        private Context _context;
        private IEngineService _engineService;
        private IEnumerable<Repository.Objects.Engine> _expectedEngines;
        private IEnumerable<Repository.Objects.Engine> _actualEngines;

        protected override void Arrange()
        {
            _context = ContextBuilder.Initialize().Build();

            const string s3Key = "fake s3 key";
            const string serializedObject = "this object is serialized";

            _expectedEngines = new List<Repository.Objects.Engine>
            {
                new EngineBuilder().Build(),
                new EngineBuilder().Build(),
            };

            var serialiser = A.Fake<ISerialiser>();
            var service = A.Fake<IService>();
            var keyManager = A.Fake<IKeyManager>();

            A.CallTo(() => keyManager.GetEnginesKey(A<Guid>._, A<Guid>._)).Returns(s3Key);
            A.CallTo(() => service.GetObject(_context.Brand, _context.Country, s3Key)).Returns(serializedObject);
            A.CallTo(() => serialiser.Deserialise<IEnumerable<Repository.Objects.Engine>>(serializedObject)).Returns(_expectedEngines);

            var serviceFacade = new S3ServiceFacade()
                .WithService(service)
                .WithSerializer(serialiser)
                .WithKeyManager(keyManager);

            _engineService = serviceFacade.CreateEngineService();
        }

        protected override void Act()
        {
            _actualEngines = _engineService.GetEngines(Guid.NewGuid(), Guid.NewGuid(), _context);
        }

        [Fact]
        public void ThenItShouldReturnTheCorrectListOfEngines()
        {
            _actualEngines.Should().BeSameAs(_expectedEngines);
        }
    }
}