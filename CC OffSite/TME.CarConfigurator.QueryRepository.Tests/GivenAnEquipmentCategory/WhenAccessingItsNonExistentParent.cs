using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using System;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Repository.Objects.Core;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CategoryConfigurator.Query.Tests.GivenAnEquipmentCategory
{
    public class WhenAccessingItsNonExistentParent : TestBase
    {
        ICategory _category;
        ICategory _parentCategory;
        IEquipmentService _equipmentService;
        
        protected override void Arrange()
        {
            var repoCategory = new EquipmentCategoryBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            _equipmentService = A.Fake<IEquipmentService>();
            A.CallTo(() => _equipmentService.GetCategories(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new [] { repoCategory });

            var categoryFactory = new EquipmentFactoryBuilder()
                .WithEquipmentService(_equipmentService)
                .Build();

            _category = categoryFactory.GetCategories(publication, context).Single();
        }

        protected override void Act()
        {
            _parentCategory = _category.Parent;
        }

        [Fact]
        public void ThenTheParentEquipmentShouldBeCorrect()
        {
            _parentCategory.ID.Should().Be(Guid.Empty);
        }
    }
}
