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

namespace TME.CarConfigurator.Query.Tests.GivenAnEngineType
{
    public class WhenAccessingItsFuelTypeForTheFirstTime : TestBase
    {
        IEngineType _engineType;
        IFuelType _fuelType;
        Repository.Objects.FuelType _repoFuelType;

        protected override void Arrange()
        {
            _repoFuelType = new FuelTypeBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoEngineType = new EngineTypeBuilder()
                .WithFuelType(_repoFuelType)
                .Build();

            _engineType = new EngineType(repoEngineType);
        }

        protected override void Act()
        {
            _fuelType = _engineType.FuelType;
        }

        [Fact]
        public void ThenItShouldHaveTheFuelType()
        {
            _fuelType.ID.Should().Be(_repoFuelType.ID);
        }
    }
}
