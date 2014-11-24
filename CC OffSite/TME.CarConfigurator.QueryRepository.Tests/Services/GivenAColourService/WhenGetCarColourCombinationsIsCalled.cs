using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.DI;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.Services.GivenAColourService
{
    public class WhenGetCarColourCombinationsIsCalled : TestBase
    {
        private Context _context;
        private IColourService _colourService;
        private IEnumerable<CarColourCombination> _expectedColours;
        private IEnumerable<CarColourCombination> _actualColours;

        protected override void Arrange()
        {
            _context = ContextBuilder.Initialize().Build();

            const string s3Key = "fake s3 key";
            const string serializedObject = "this object is serialized";

            _expectedColours = new List<CarColourCombination>
            {
                new CarColourCombinationBuilder().Build(),
                new CarColourCombinationBuilder().Build(),
            };

            var serialiser = A.Fake<ISerialiser>();
            var service = A.Fake<IService>();
            var keyManager = A.Fake<IKeyManager>();

            A.CallTo(() => keyManager.GetCarColourCombinationsKey(A<Guid>._, A<Guid>._)).Returns(s3Key);
            A.CallTo(() => service.GetObject(_context.Brand, _context.Country, s3Key)).Returns(serializedObject);
            A.CallTo(() => serialiser.Deserialise<IEnumerable<CarColourCombination>>(serializedObject)).Returns(_expectedColours);

            var serviceFacade = new S3ServiceFacade()
                .WithService(service)
                .WithSerializer(serialiser)
                .WithKeyManager(keyManager);

            _colourService = serviceFacade.CreateColourService();
        }

        protected override void Act()
        {
            _actualColours = _colourService.GetCarColourCombinations(Guid.NewGuid(), _context, Guid.NewGuid());
        }

        [Fact]
        public void ThenItShouldReturnTheCorrectListOfColours()
        {
            _actualColours.Should().BeSameAs(_expectedColours);
        }
    }
}