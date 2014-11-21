using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using TME.CarConfigurator.DI;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.Services.GivenAnEquipmentService
{
    public class WhenGetEquipmentCategoriesIsCalled : TestBase
    {
        private IEnumerable<Category> _actualCategories;
        private IEnumerable<Category> _expectedCategories;
        private IEquipmentService _equipmentService;
        private Context _context;

        protected override void Arrange()
        {
            _context = new ContextBuilder().Build();

            const string s3Key = "fake s3 key";
            const string serializedObject = "this object is serialized";

            _expectedCategories = new [] {
                new EquipmentCategoryBuilder().Build(),
                new EquipmentCategoryBuilder().Build()
            };

            var serialiser = A.Fake<ISerialiser>();
            var service = A.Fake<IService>();
            var keyManager = A.Fake<IKeyManager>();

            A.CallTo(() => keyManager.GetEquipmentCategoriesKey(A<Guid>._)).Returns(s3Key);
            A.CallTo(() => service.GetObject(_context.Brand, _context.Country, s3Key)).Returns(serializedObject);
            A.CallTo(() => serialiser.Deserialise<IEnumerable<Category>>(serializedObject)).Returns(_expectedCategories);

            var serviceFacade = new S3ServiceFacade()
                .WithService(service)
                .WithSerializer(serialiser)
                .WithKeyManager(keyManager);

            _equipmentService = serviceFacade.CreateEquipmentService();
        }

        protected override void Act()
        {
            _actualCategories = _equipmentService.GetCategories(Guid.NewGuid(), _context);
        }

        [Fact]
        public void ThenItShouldReturnTheCorrectListOfCategories()
        {
            _actualCategories.Should().BeSameAs(_expectedCategories);
        }
    }
}
