using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAModelEquipment
{
    public class WhenAccessingItsCategoriesForTheSecondTime : TestBase
    {
        private IEnumerable<ICategory> _firstCategories;
        private IEnumerable<ICategory> _secondCategories;
        private Repository.Objects.Equipment.Category _category1;
        private Repository.Objects.Equipment.Category _category2;
        private IEquipmentService _equipmentService;
        private IModelEquipment _modelEquipment;

        protected override void Arrange()
        {
            _category1 = new EquipmentCategoryBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _category2 = new EquipmentCategoryBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder()
                .WithID(Guid.NewGuid())
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            _equipmentService = A.Fake<IEquipmentService>();
            A.CallTo(() => _equipmentService.GetCategories(publication.ID, publicationTimeFrame.ID, context)).Returns(new [] { _category1, _category2 });

            var equipmentFactory = new EquipmentFactoryBuilder()
                .WithEquipmentService(_equipmentService)
                .Build();

            _modelEquipment = equipmentFactory.GetModelEquipment(publication, context);

            _firstCategories = _modelEquipment.Categories;
        }

        protected override void Act()
        {
            _secondCategories = _modelEquipment.Categories;
        }

        [Fact]
        public void ThenItShouldNotFetchTheCategoriesFromTheServiceAgain()
        {
            A.CallTo(() => _equipmentService.GetCategories(A<Guid>._, A<Guid>._, A<Context>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenTheListOfEnginesShouldBeCorrect()
        {
            _secondCategories.Should().BeSameAs(_firstCategories);
        }
    }
}
