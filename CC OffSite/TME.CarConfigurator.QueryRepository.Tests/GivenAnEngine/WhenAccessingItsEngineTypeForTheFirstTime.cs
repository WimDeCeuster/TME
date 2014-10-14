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
    public class WhenAccessingItsEngineTypeForTheFirstTime : TestBase
    {
        IEngine _engine;
        IEngineType _type;
        Repository.Objects.EngineType _repoType;

        protected override void Arrange()
        {
            _repoType = new EngineTypeBuilder()
                .WithCode("engine type code")
                .Build();

            var repoEngine = new EngineBuilder()
                .WithType(_repoType)
                .Build();

            _engine = new Engine(repoEngine);
        }

        protected override void Act()
        {
            _type = _engine.Type;
        }

        [Fact]
        public void ThenItShouldHaveTheType()
        {
            _type.Code.Should().Be(_repoType.Code);
        }
    }
}
