using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces.TechnicalSpecifications;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAModelSpecifications
{
    public class WhenAccessingItsCategoriesForTheFirstTime : TestBase
    {
        private IEnumerable<ICategory> _categories;
        private Repository.Objects.TechnicalSpecifications.Category _category1;
        private Repository.Objects.TechnicalSpecifications.Category _category2;
        private ISpecificationsService _equipmentService;
        private IModelTechnicalSpecifications _modelSpecifications;

        protected override void Arrange()
        {
            _category1 = new SpecificationCategoryBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _category2 = new SpecificationCategoryBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .Build();

            var context = new ContextBuilder().Build();

            _equipmentService = A.Fake<ISpecificationsService>();
            A.CallTo(() => _equipmentService.GetCategories(publication.ID, context)).Returns(new [] { _category1, _category2 });

            var equipmentFactory = new SpecificationsFactoryBuilder()
                .WithSpecificationsService(_equipmentService)
                .Build();

            _modelSpecifications = equipmentFactory.GetModelSpecifications(publication, context);
        }

        protected override void Act()
        {
            _categories = _modelSpecifications.Categories;
        }

        [Fact]
        public void ThenItShouldFetchTheCategoriesFromTheService()
        {
            A.CallTo(() => _equipmentService.GetCategories(A<Guid>._, A<Context>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldHaveTheCorrectCategories()
        {
            _categories.Should().HaveCount(2);

            _categories.Should().Contain(a => a.ID == _category1.ID);
            _categories.Should().Contain(a => a.ID == _category2.ID);
        }
    }
}
