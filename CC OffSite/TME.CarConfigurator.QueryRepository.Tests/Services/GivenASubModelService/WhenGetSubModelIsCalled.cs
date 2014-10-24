using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.DI;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.QueryServices;
using TME.CarConfigurator.S3.Shared;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.Services.GivenASubModelService
{
    public class WhenGetSubModelIsCalled : TestBase
    {
        private ISubModelService _subModelService;
        private Context _context;
        private IEnumerable<Repository.Objects.SubModel> _actualSubModels;
        private IEnumerable<Repository.Objects.SubModel> _expectedSubModels;

        protected override void Arrange()
        {
            _context = ContextBuilder.Initialize().Build();

            const string s3Key = "fake s3 key";
            const string serialisedObject = "serialised objects";

            _expectedSubModels = new List<Repository.Objects.SubModel>()
            {
                new SubModelBuilder().Build(),
                new SubModelBuilder().Build(),
            };

            var service = A.Fake<IService>();
            var serialiser = A.Fake<ISerialiser>();
            var keyManager = A.Fake<IKeyManager>();

            A.CallTo(() => keyManager.GetSubModelsKey(A<Guid>._, A<Guid>._)).Returns(s3Key);
            A.CallTo(() => service.GetObject(_context.Brand, _context.Country, s3Key)).Returns(serialisedObject);
            A.CallTo(() => serialiser.Deserialise<IEnumerable<Repository.Objects.SubModel>>(serialisedObject)).Returns(_expectedSubModels);

            var serviceFacade = new S3ServiceFacade()
                .WithService(service)
                .WithSerializer(serialiser)
                .WithKeyManager(keyManager);

            _subModelService = serviceFacade.CreateSubModelService();
        }

        protected override void Act()
        {
            _actualSubModels = _subModelService.GetSubModels(Guid.NewGuid(), Guid.NewGuid(), _context);
        }

        [Fact]
        public void ThenItShouldReturnTheCorrectListOfSubModels()
        {
            _actualSubModels.Should().BeSameAs(_expectedSubModels);
        }
    }
}