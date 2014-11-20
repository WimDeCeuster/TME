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

namespace TME.CarConfigurator.Query.Tests.GivenAModelSpecification
{
    public class WhenAccessingItsCategoriesForTheSecondTime : TestBase
    {
        private IEnumerable<ICategory> _firstCategories;
        private IEnumerable<ICategory> _secondCategories;
        private Repository.Objects.TechnicalSpecifications.Category _category1;
        private Repository.Objects.TechnicalSpecifications.Category _category2;
        private ISpecificationsService _specificationService;
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

            _specificationService = A.Fake<ISpecificationsService>();
            A.CallTo(() => _specificationService.GetCategories(publication.ID, context)).Returns(new [] { _category1, _category2 });

            var specificationsFactory = new SpecificationsFactoryBuilder()
                .WithSpecificationsService(_specificationService)
                .Build();

            _modelSpecifications = specificationsFactory.GetModelSpecifications(publication, context);

            _firstCategories = _modelSpecifications.Categories;
        }

        protected override void Act()
        {
            _secondCategories = _modelSpecifications.Categories;
        }

        [Fact]
        public void ThenItShouldNotFetchTheCategoriesFromTheServiceAgain()
        {
            A.CallTo(() => _specificationService.GetCategories(A<Guid>._, A<Context>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenTheListOfEnginesShouldBeCorrect()
        {
            _secondCategories.Should().BeSameAs(_firstCategories);
        }
    }
}
