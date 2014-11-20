using System;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.Repository.Objects.Extensions;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAnEquipmentCategory
{
    public class WhenAccessingItsParentForTheFirstTime : TestBase
    {
        ICategory _category;
        ICategory _parentCategory;
        Category _repoParentCategory;

        protected override void Arrange()
        {
            _repoParentCategory = new EquipmentCategoryBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoCategory = new EquipmentCategoryBuilder()
                .WithId(Guid.NewGuid())
                .WithParentId(_repoParentCategory.ID)
                .Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            var equipmentService = A.Fake<IEquipmentService>();
            A.CallTo(() => equipmentService.GetCategories(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new [] { repoCategory, _repoParentCategory });

            var categoryFactory = new EquipmentFactoryBuilder()
                .WithEquipmentService(equipmentService)
                .Build();

            _category = categoryFactory.GetCategories(publication, context).ToList().Flatten(category => category.Categories.ToList()).Single(category => category.ID == repoCategory.ID);
        }

        protected override void Act()
        {
            _parentCategory = _category.Parent;
        }

        [Fact]
        public void ThenTheBasedUponCategoryShouldBeCorrect()
        {
            _parentCategory.ID.Should().Be(_repoParentCategory.ID);
        }
    }
}
