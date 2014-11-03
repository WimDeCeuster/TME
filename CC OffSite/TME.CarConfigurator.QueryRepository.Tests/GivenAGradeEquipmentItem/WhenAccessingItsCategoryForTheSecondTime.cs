using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAGradeEquipmentItem
{
    public class WhenAccessingItsCategoryForTheSecondTime : TestBase
    {
        ICategoryInfo _firstCategory;
        ICategoryInfo _secondCategory;
        Repository.Objects.Equipment.CategoryInfo _repoCategory;
        IGradeEquipmentItem _accessory;

        protected override void Arrange()
        {
            _repoCategory = new CarConfigurator.Tests.Shared.TestBuilders.CategoryInfoBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoAccessory = new GradeAccessoryBuilder()
                .WithCategory(_repoCategory)
                .Build();

            var repoGradeEquipment = new GradeEquipmentBuilder()
                .WithAccessories(repoAccessory)
                .Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            var gradeEquipmentService = A.Fake<IEquipmentService>();
            A.CallTo(() => gradeEquipmentService.GetGradeEquipment(A<Guid>._, A<Guid>._, A<Guid>._, A<Context>._)).Returns(repoGradeEquipment);

            var gradeEquipmentFactory = new EquipmentFactoryBuilder()
                .WithEquipmentService(gradeEquipmentService)
                .Build();

            _accessory = gradeEquipmentFactory.GetGradeEquipment(publication, context, Guid.Empty).Accessories.Single();

            _firstCategory = _accessory.Category;
        }

        protected override void Act()
        {
            _secondCategory = _accessory.Category;
        }

        [Fact]
        public void ThenItShouldNotRecalculateTheCategory()
        {
            _secondCategory.Should().Be(_firstCategory);
        }

        [Fact]
        public void ThenItShouldHaveTheCategory()
        {
            _secondCategory.ID.Should().Be(_repoCategory.ID);
        }


    }
}
