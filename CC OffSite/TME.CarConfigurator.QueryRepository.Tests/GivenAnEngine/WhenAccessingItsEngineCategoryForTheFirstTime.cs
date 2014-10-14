using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAnEngine
{
    public class WhenAccessingItsEngineCategoryForTheFirstTime : TestBase
    {
        IEngine _engine;
        IEngineCategory _category;
        Repository.Objects.EngineCategory _repoCategory;

        protected override void Arrange()
        {
            _repoCategory = new EngineCategoryBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoEngine = new EngineBuilder()
                .WithCategory(_repoCategory)
                .Build();

            _engine = new Engine(repoEngine);
        }

        protected override void Act()
        {
            _category = _engine.Category;
        }

        [Fact]
        public void ThenItShouldHaveTheCategory()
        {
            _category.ID.Should().Be(_repoCategory.ID);
        }
    }
}
