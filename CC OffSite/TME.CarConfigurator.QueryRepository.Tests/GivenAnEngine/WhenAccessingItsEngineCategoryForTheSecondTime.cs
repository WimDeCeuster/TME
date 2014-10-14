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
    public class WhenAccessingItsEngineCategoryForTheSecondTime : TestBase
    {
        IEngine _engine;
        IEngineCategory _firstCategory;
        IEngineCategory _secondCategory;
        Repository.Objects.EngineCategory _repoCategory;

        protected override void Arrange()
        {
            _repoCategory = new EngineCategoryBuilder().Build();

            var repoEngine = new EngineBuilder()
                .WithCategory(_repoCategory)
                .Build();

            _engine = new Engine(repoEngine);

            _firstCategory = _engine.Category;
        }

        protected override void Act()
        {
            _secondCategory = _engine.Category;
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
