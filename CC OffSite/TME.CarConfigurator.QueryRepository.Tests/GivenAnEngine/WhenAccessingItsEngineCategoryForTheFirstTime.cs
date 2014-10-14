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

            var publicationTimeFrame = new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            var engineService = A.Fake<IEngineService>();
            A.CallTo(() => engineService.GetEngines(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new List<Repository.Objects.Engine> { repoEngine });

            var engineFactory = new EngineFactoryBuilder()
                .WithEngineService(engineService)
                .Build();

            _engine = engineFactory.GetEngines(publication, context).Single();
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
